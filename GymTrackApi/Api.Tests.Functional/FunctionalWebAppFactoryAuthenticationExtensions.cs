using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Common.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Tests.Functional;

internal static class FunctionalWebAppFactoryAuthenticationExtensions
{
	public static Task<HttpClient> CreateLoggedInUserClient(this FunctionalTestWebAppFactory factory)
	{
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
		return factory.CreateLoggedInUserClient(email);
	}

	public static async Task<HttpClient> CreateLoggedInUserClient(this FunctionalTestWebAppFactory factory, EmailAddress email)
	{
		var password = Password.From("User!123");

		// REGISTER

		var httpClient = factory.CreateClient();
		var response = await httpClient.PostAsJsonAsync("auth/register", new RegisterRequest(
				email.Value,
				password.Value))
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		// LOG IN

		response = await httpClient.PostAsJsonAsync("auth/login", new LoginRequest(email.Value, password.Value));

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

		var tokens = await response.Content.ReadFromJsonAsync<LoginResponse>().ConfigureAwait(false);
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
			JwtBearerDefaults.AuthenticationScheme,
			tokens!.AccessToken);

		// CONFIRM EMAIL

		var confirmationCode = await FakeUserEmailSenderCache.GetEmailConfirmationCode(email);

		response = await httpClient.PostAsJsonAsync(
				"auth/confirm-email",
				new ConfirmEmailRequest(confirmationCode.Value))
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		// SET ANTIFORGERY TOKEN HEADER

		response = await httpClient.GetAsync("auth/antiforgery-token");
		var antiforgeryToken = await response.Content.ReadFromJsonAsync<GetAntiforgeryTokenResponse>();
		httpClient.DefaultRequestHeaders.Add(antiforgeryToken!.HeaderName, antiforgeryToken.RequestToken);
		await Assert.That(response.Headers.GetValues("Set-Cookie").First()).IsNotNull();

		return httpClient;
	}
}