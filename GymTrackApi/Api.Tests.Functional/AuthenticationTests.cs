using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Common.ValueObjects;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Functional;

internal sealed class AuthenticationTests
{
	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task RegisterAndLogin_ValidUser_ReturnsCorrectResponse(FunctionalTestWebAppFactory factory)
	{
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		var client = await factory.CreateLoggedInUserClient(email);

		// Verify user is properly created in DB
		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users.FirstAsync(user => user.Email == email);
		await Assert.That(user.HasConfirmedEmail).IsTrue();
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Register_ValidRequest_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		var response = await client.PostAsJsonAsync("auth/register", new RegisterRequest(email.Value, "ValidPassword123!"));

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		// Verify user creation in database
		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
		await Assert.That(user).IsNotNull();
		await Assert.That(user!.HasConfirmedEmail).IsFalse();
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Register_InvalidEmail_ReturnsBadRequest(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var response = await client.PostAsJsonAsync("auth/register",
			new RegisterRequest("invalid-email", "ValidPassword123!"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ConfirmEmail_ValidCode_ConfirmsEmail(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		var password = Password.From("Password1!");
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email.Value, password.Value));

		var response = await client.PostAsJsonAsync("auth/login", new LoginRequest(email.Value, password.Value));

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		var tokens = await response.Content.ReadFromJsonAsync<LoginResponse>().ConfigureAwait(false);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
			JwtBearerDefaults.AuthenticationScheme,
			tokens!.AccessToken);

		var confirmationCode = await FakeUserEmailSenderCache.GetEmailConfirmationCode(email).ConfigureAwait(false);
		response = await client.PostAsJsonAsync("auth/confirm-email",
			new ConfirmEmailRequest(confirmationCode.Value));

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var updatedUser = await dbContext.Users.FirstAsync(user => user.Email == email);
		await Assert.That(updatedUser.HasConfirmedEmail).IsTrue();
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task RefreshLogin_InvalidToken_ReturnsUnauthorized(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.PostAsJsonAsync("auth/refresh-login",
			new RefreshLoginRequest("invalid_refresh_token"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task InvalidateRefreshTokens_InvalidatesTokens(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var refreshToken = client.DefaultRequestHeaders.Authorization!.Parameter;

		var invalidateResponse = await client.PostAsync("auth/invalidate-refresh-tokens", null);
		await Assert.That(invalidateResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		var refreshResponse = await client.PostAsJsonAsync("auth/refresh-login",
			new RefreshLoginRequest(refreshToken!));
		await Assert.That(refreshResponse.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ResetPassword_InvalidCode_ReturnsBadRequest(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.PostAsJsonAsync("auth/reset-password",
			new ResetPasswordRequest("INVALID_CODE", "NewPassword123!"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ResetPassword_ValidCode_ResetsPassword(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		const string originalPassword = "OriginalPassword123!";
		const string newPassword = "NewPassword123!";

		// Register user
		await client.PostAsJsonAsync("auth/register",
			new RegisterRequest(email.Value, originalPassword));

		// Trigger forgot password
		await client.PostAsJsonAsync("auth/forgot-password",
			new ForgotPasswordRequest(email.Value));

		// Get reset code from database
		var resetCode = await FakeUserEmailSenderCache.GetPasswordResetCode(email).ConfigureAwait(false);

		// Reset password with valid code
		var resetClient = factory.CreateClient(); // Fresh unauthenticated client
		var resetResponse = await resetClient.PostAsJsonAsync("auth/reset-password",
			new ResetPasswordRequest(resetCode.Value, newPassword));

		await Assert.That(resetResponse.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		// Verify password was changed
		var loginResponseWithOldPassword = await client.PostAsJsonAsync("auth/login",
			new LoginRequest(email.Value, originalPassword));
		await Assert.That(loginResponseWithOldPassword.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

		var loginResponseWithNewPassword = await client.PostAsJsonAsync("auth/login",
			new LoginRequest(email.Value, newPassword));
		await Assert.That(loginResponseWithNewPassword.StatusCode).IsEqualTo(HttpStatusCode.OK);

		// Verify reset code was cleared
		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users
			.Include(user => user.PasswordResetCodes)
			.FirstAsync(user => user.Email == email);

		await Assert.That(user.PasswordResetCodes.Count).IsEqualTo(0);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ForgotPassword_GeneratesResetCode(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email.Value, "Password1!"));

		await client.PostAsJsonAsync("auth/forgot-password", new ForgotPasswordRequest(email.Value));

		var sentCode = await FakeUserEmailSenderCache.GetPasswordResetCode(email);

		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users
			.Include(u => u.PasswordResetCodes)
			.FirstAsync(u => u.Email == email);

		await Assert.That(user.PasswordResetCodes[0].PasswordResetCode).IsEqualTo(sentCode);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Login_InvalidCredentials_ReturnsUnauthorized(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = $"{Guid.NewGuid()}@user.com";
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email, "Password1!"));

		var response = await client.PostAsJsonAsync("auth/login",
			new LoginRequest(email, "WrongPassword!"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task AntiforgeryToken_SetsCookieHeader(FunctionalTestWebAppFactory factory)
	{
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		var password = Password.From("User!123");

		// register
		var client = factory.CreateClient();
		var response = await client.PostAsJsonAsync("auth/register", new RegisterRequest(
				email.Value,
				password.Value))
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		// log in
		response = await client.PostAsJsonAsync("auth/login", new LoginRequest(email.Value, password.Value));

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		var tokens = await response.Content.ReadFromJsonAsync<LoginResponse>().ConfigureAwait(false);
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
			JwtBearerDefaults.AuthenticationScheme,
			tokens!.AccessToken);

		// get tokens
		response = await client.GetAsync("auth/antiforgery-token");

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
		await Assert.That(response.Headers.Contains("Set-Cookie")).IsTrue();
		var cookie = response.Headers.GetValues("Set-Cookie").First();
		await Assert.That(cookie).Contains("Antiforgery");
	}
}