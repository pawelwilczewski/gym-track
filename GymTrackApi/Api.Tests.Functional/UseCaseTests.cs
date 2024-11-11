using System.Net;
using System.Net.Http.Json;
using Api.Dtos;

namespace Api.Tests.Functional;

internal sealed class UseCaseTests
{
	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task CreateWorkout_Valid_Succeeds(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Nice Workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(response.Headers.Location!).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();
	}
}