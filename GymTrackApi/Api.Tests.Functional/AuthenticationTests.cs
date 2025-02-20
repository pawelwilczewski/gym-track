using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Common.ValueObjects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Functional;

internal sealed class AuthenticationTests
{
	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public Task RegisterAndLogin_ValidUser_ReturnsCorrectResponse(FunctionalTestWebAppFactory factory) =>
		factory.CreateLoggedInUserClient();

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Register_ValidRequest_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var request = new RegisterRequest($"{Guid.NewGuid()}@user.com", "ValidPassword123!");
		var response = await client.PostAsJsonAsync("auth/register", request);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Register_DuplicateEmail_ReturnsConflict(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = $"{Guid.NewGuid()}@user.com";
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email, "Password1!"));
		var response = await client.PostAsJsonAsync("auth/register", new RegisterRequest(email, "Password2!"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task Register_InvalidRequest_ReturnsBadRequest(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var response = await client.PostAsJsonAsync("auth/register", new
			{ });
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task RefreshLogin_ValidToken_ReturnsNewTokens(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		var password = "ValidPassword123!";

		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email.Value, password));
		var loginResponse = await client.PostAsJsonAsync("auth/login", new LoginRequest(email.Value, password));
		var loginTokens = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users.Include(u => u.EmailConfirmationCode).FirstAsync(u => u.Email == email);
		await client.PostAsJsonAsync("auth/confirm-email", new ConfirmEmailRequest(user.EmailConfirmationCode!.EmailConfirmationCode.Value));

		var refreshResponse = await client.PostAsJsonAsync("auth/refresh-login", new RefreshLoginRequest(loginTokens!.RefreshToken));
		await Assert.That(refreshResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);
		var newTokens = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();
		await Assert.That(newTokens?.AccessToken).IsNotNull();
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task InvalidateRefreshTokens_ValidRequest_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.PostAsync("auth/invalidate-refresh-tokens", null);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ConfirmEmail_InvalidCode_ReturnsBadRequest(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.PostAsJsonAsync("auth/confirm-email", new ConfirmEmailRequest("INVALID_CODE"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task SendConfirmationEmail_ValidRequest_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.PostAsJsonAsync("auth/send-confirmation-email", new
			{ });
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ForgotPassword_ValidEmail_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = $"{Guid.NewGuid()}@user.com";
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email, "Password1!"));
		var response = await client.PostAsJsonAsync("auth/forgot-password", new ForgotPasswordRequest(email));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task ResetPassword_ValidRequest_ReturnsNoContent(FunctionalTestWebAppFactory factory)
	{
		var client = factory.CreateClient();
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		await client.PostAsJsonAsync("auth/register", new RegisterRequest(email.Value, "Password1!"));

		await client.PostAsJsonAsync("auth/forgot-password", new ForgotPasswordRequest(email.Value));

		using var scope = factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dbContext.Users
			.Include(u => u.PasswordResetCode)
			.FirstAsync(u => u.Email == email);

		var response = await client.PostAsJsonAsync("auth/reset-password",
			new ResetPasswordRequest(user.PasswordResetCode!.PasswordResetCode.Value, "NewPassword123!"));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task GetAntiforgeryToken_ValidRequest_ReturnsToken(FunctionalTestWebAppFactory factory)
	{
		var client = await factory.CreateLoggedInUserClient();
		var response = await client.GetAsync("auth/antiforgery-token");
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		var tokenResponse = await response.Content.ReadFromJsonAsync<GetAntiforgeryTokenResponse>();
		await Assert.That(tokenResponse?.RequestToken).IsNotNull();
	}
}