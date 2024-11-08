using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity.Data;

namespace Api.Tests.Functional;

internal sealed class AuthenticationTests
{
	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task RegisterAndLogin_ValidUser_ReturnsCorrectResponse(FunctionalTestWebAppFactory factory)
	{
		var httpClient = factory.CreateClient();
		var response = await httpClient.PostAsJsonAsync("auth/register", new RegisterRequest
			{
				Email = "user@user.com",
				Password = "User!123"
			})
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
	}
}