using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using Api.Dtos;
using Domain.Models;
using Domain.Models.Workout;

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

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task CreateWorkout_InvalidEdit_Fails(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Nice Workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		var workoutUri = response.Headers.Location!;
		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(workoutUri).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();

		response = await httpClient.PutAsJsonAsync(workoutUri, new EditWorkoutRequest(" ,,,,")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task CreateAndEditMultipleAssets_Valid_Succeeds(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Nice Workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var workoutUri = response.Headers.Location!;

		var content = new MultipartFormDataContent();

		content.Add(new StringContent("Nice Exercise"), "name");
		content.Add(new StringContent("Some exercise description."), "description");
		content.Add(new StringContent(((int)(ExerciseMetricType.Distance | ExerciseMetricType.Weight)).ToString()), "allowedMetricTypes");

		var imageStream = new MemoryStream();
		var writer = new StreamWriter(imageStream);
		await writer.WriteAsync("Test jpg image content").ConfigureAwait(false);
		await writer.FlushAsync().ConfigureAwait(false);
		var imageContent = new StreamContent(imageStream);
		imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
		content.Add(imageContent, "thumbnailImage", "ExampleExerciseInfoThumbnail.jpg");

		response = await httpClient.PostAsync("api/v1/exerciseInfos", content).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var exerciseUri = response.Headers.Location!;
		var exerciseUriString = exerciseUri.ToString();
		var lastSlashIndex = exerciseUriString.LastIndexOf('/');
		var exerciseId = Guid.Parse(exerciseUriString[(lastSlashIndex + 1)..]);

		response = await httpClient.PostAsJsonAsync($"{workoutUri}/exercises", new CreateWorkoutExerciseRequest(0, exerciseId));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		Amount.TryCreate(1000.0, out var distance);
		response = await httpClient.PostAsJsonAsync($"{workoutUri}/exercises/0/sets", new CreateWorkoutExerciseSetRequest(0, new Distance(distance, Distance.Unit.Metre), 3));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var setUri = response.Headers.Location!;

		var set = await httpClient.GetFromJsonAsync<GetWorkoutExerciseSetResponse>(setUri).ConfigureAwait(false);
		await Assert.That(set).IsNotNull();

		Amount.TryCreate(30.0, out var weight);
		response = await httpClient.PutAsJsonAsync($"{workoutUri}/exercises/0/sets/0", new EditWorkoutExerciseSetRequest(new Weight(weight, Weight.Unit.Kilogram), 8));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		set = await httpClient.GetFromJsonAsync<GetWorkoutExerciseSetResponse>(setUri).ConfigureAwait(false);
		await Assert.That(set).IsNotNull();
		await Assert.That(set!.Reps).IsEqualTo(8);
		await Assert.That(set.Metric is Weight weightMetric && Math.Abs(weightMetric.Value.Value - 30.0) <= 0.0001).IsTrue();
	}
}