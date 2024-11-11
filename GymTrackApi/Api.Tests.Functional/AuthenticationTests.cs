using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Api.Dtos;
using Application.Persistence;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Functional;

internal sealed class AuthenticationTests
{
	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task RegisterAndLogin_ValidUser_ReturnsCorrectResponse(FunctionalTestWebAppFactory factory)
	{
		const string email = "user@user.com";
		const string password = "User!123";

		var httpClient = factory.CreateClient();
		var response = await httpClient.PostAsJsonAsync("auth/register", new RegisterRequest
			{
				Email = email,
				Password = password
			})
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		using var scope = factory.Services.CreateScope();

		var dataContext = scope.ServiceProvider.GetRequiredService<IDataContext>();
		var user = await dataContext.Users.FirstAsync(user => user.Email == email).ConfigureAwait(false);

		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
		var code = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
		var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var query = QueryHelpers.AddQueryString("auth/confirmEmail", new Dictionary<string, string?>
		{
			{ "userId", user.Id.ToString() },
			{ "code", encodedCode }
		});

		response = await httpClient.GetAsync(query).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		response = await httpClient.PostAsJsonAsync("auth/login", new LoginRequest
		{
			Email = email,
			Password = password
		});

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		var token = await response.Content.ReadFromJsonAsync<AccessTokenResponse>().ConfigureAwait(false);
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token!.TokenType, token.AccessToken);

		response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Nice Workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(response.Headers.Location!).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();
	}
}