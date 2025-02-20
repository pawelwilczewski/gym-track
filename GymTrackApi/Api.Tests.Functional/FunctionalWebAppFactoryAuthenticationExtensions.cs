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

internal static class FunctionalWebAppFactoryAuthenticationExtensions
{
	internal static async Task<HttpClient> CreateLoggedInUserClient(this FunctionalTestWebAppFactory factory)
	{
		var email = EmailAddress.From($"{Guid.NewGuid()}@user.com");
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

		// TODO Pawel: possibly simulate this better (override email service and access "emailed" code)
		using var scope = factory.Services.CreateScope();
		var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var user = await dataContext.Users
			.Include(user => user.EmailConfirmationCode)
			.FirstAsync(user => user.Email == email)
			.ConfigureAwait(false);

		response = await httpClient.PostAsJsonAsync(
				"auth/confirm-email",
				new ConfirmEmailRequest(user.EmailConfirmationCode!.EmailConfirmationCode.Value))
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