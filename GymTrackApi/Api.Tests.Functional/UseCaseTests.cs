using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using Api.Dtos;
using Application.Tracking.TrackedWorkout.Dtos;
using Application.Workout.Dtos;
using Application.Workout.Exercise.Dtos;
using Application.Workout.Exercise.Set.Dtos;
using Domain.Models;
using Domain.Models.ExerciseInfo;

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
	public async Task CreateWorkout_InvalidUpdate_Fails(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Nice Workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		var workoutUri = response.Headers.Location!;
		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(workoutUri).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();

		response = await httpClient.PutAsJsonAsync(workoutUri, new UpdateWorkoutRequest(" ,,,,")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
	}

	private async Task<Guid> CreateExerciseInfo(HttpClient httpClient, GetAntiforgeryTokenResponse antiforgeryToken)
	{
		var content = new MultipartFormDataContent();
		content.Add(new StringContent(antiforgeryToken.Token), "__RequestVerificationToken");
		content.Add(new StringContent("Nice Exercise"), "name");
		content.Add(new StringContent("Some exercise description."), "description");
		content.Add(new StringContent(((int)(ExerciseMetricType.Distance | ExerciseMetricType.Weight)).ToString()), "allowedMetricTypes");

		var imageStream = new MemoryStream();
		var writer = new StreamWriter(imageStream);
		await writer.WriteAsync("Test jpg image content").ConfigureAwait(false);
		await writer.FlushAsync().ConfigureAwait(false);
		imageStream.Position = 0;
		var imageContent = new StreamContent(imageStream);
		imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
		content.Add(imageContent, "thumbnailImage", "ExampleExerciseInfoThumbnail.jpg");

		var response = await httpClient.PostAsync("api/v1/exercise-infos", content).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var exerciseInfoUri = response.Headers.Location!;
		var exerciseInfoUriString = exerciseInfoUri.ToString();
		var lastSlashIndex = exerciseInfoUriString.LastIndexOf('/');
		return Guid.Parse(exerciseInfoUriString[(lastSlashIndex + 1)..]);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task CreateUpdateDeleteMultipleAssets_Valid_Succeeds(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var antiforgeryToken = await httpClient.GetFromJsonAsync<GetAntiforgeryTokenResponse>("auth/antiforgery-token");
		var exerciseId = await CreateExerciseInfo(httpClient, antiforgeryToken!).ConfigureAwait(false);

		const string workoutName = "Nice Workout";
		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest(workoutName)).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var workoutUri = response.Headers.Location!;

		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(workoutUri).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();
		await Assert.That(workout!.Name).IsEqualTo(workoutName);

		const int exerciseIndex = 0;
		response = await httpClient.PostAsJsonAsync($"{workoutUri}/exercises", new CreateWorkoutExerciseRequest(exerciseId));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var exerciseUri = response.Headers.Location!;

		var exercise = await httpClient.GetFromJsonAsync<GetWorkoutExerciseResponse>(exerciseUri).ConfigureAwait(false);
		await Assert.That(exercise).IsNotNull();
		await Assert.That(exercise!.Index).IsEqualTo(exerciseIndex);

		const int setIndex = 0;
		var distance = WeightValue.From(1000.0);
		response = await httpClient.PostAsJsonAsync($"{workoutUri}/exercises/{exerciseIndex}/sets", new CreateWorkoutExerciseSetRequest(new Distance(distance, Distance.Unit.Metre), 3));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var setUri = response.Headers.Location!;

		var set = await httpClient.GetFromJsonAsync<GetWorkoutExerciseSetResponse>(setUri).ConfigureAwait(false);
		await Assert.That(set).IsNotNull();

		var weight = WeightValue.From(30.0);
		response = await httpClient.PutAsJsonAsync($"{workoutUri}/exercises/{exerciseIndex}/sets/{setIndex}", new UpdateWorkoutExerciseSetRequest(new Weight(weight, Weight.Unit.Kilogram), 8));
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		set = await httpClient.GetFromJsonAsync<GetWorkoutExerciseSetResponse>(setUri).ConfigureAwait(false);
		await Assert.That(set).IsNotNull();
		await Assert.That(set!.Reps).IsEqualTo(8);
		await Assert.That(set.Metric is Weight weightMetric && Math.Abs(weightMetric.Value.Value - 30.0) <= 0.0001).IsTrue();

		response = await httpClient.DeleteAsync(workoutUri).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		await Assert.That((await httpClient.GetAsync(setUri).ConfigureAwait(false)).StatusCode).IsEqualTo(HttpStatusCode.NotFound);
		await Assert.That((await httpClient.GetAsync(exerciseUri).ConfigureAwait(false)).StatusCode).IsEqualTo(HttpStatusCode.NotFound);
		await Assert.That((await httpClient.GetAsync(workoutUri).ConfigureAwait(false)).StatusCode).IsEqualTo(HttpStatusCode.NotFound);
	}

	[Test]
	[ClassDataSource<FunctionalTestWebAppFactory>(Shared = SharedType.PerTestSession)]
	public async Task CreateAndUpdateTrackedWorkouts_Valid_Succeeds(FunctionalTestWebAppFactory factory)
	{
		var httpClient = await factory.CreateLoggedInUserClient().ConfigureAwait(false);

		var antiforgeryToken = await httpClient.GetFromJsonAsync<GetAntiforgeryTokenResponse>("auth/antiforgery-token");
		var exerciseId = await CreateExerciseInfo(httpClient, antiforgeryToken!);

		var response = await httpClient.PostAsJsonAsync("api/v1/workouts", new CreateWorkoutRequest("Test workout")).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
		var workoutUri = response.Headers.Location!;
		var workout = await httpClient.GetFromJsonAsync<GetWorkoutResponse>(workoutUri).ConfigureAwait(false);
		await Assert.That(workout).IsNotNull();

		var performedAt = DateTime.Now;
		var duration = TimeSpan.FromMinutes(30.0);
		response = await httpClient.PostAsJsonAsync("api/v1/tracking/workouts",
				new CreateTrackedWorkoutRequest(workout!.Id, performedAt, duration))
			.ConfigureAwait(false);

		var trackedWorkoutUri = response.Headers.Location!;
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

		var trackedWorkout = await httpClient.GetFromJsonAsync<GetTrackedWorkoutResponse>(response.Headers.Location!).ConfigureAwait(false);

		await Assert.That(trackedWorkout).IsNotNull();
		await Assert.That(trackedWorkout!.WorkoutId).IsEqualTo(workout.Id);
		await Assert.That(trackedWorkout.PerformedAt - performedAt).IsLessThan(TimeSpan.FromSeconds(0.1));
		await Assert.That(trackedWorkout.Duration).IsEqualTo(duration);

		performedAt = DateTime.Today - TimeSpan.FromDays(1);
		duration = TimeSpan.FromMinutes(40.0);
		response = await httpClient.PutAsJsonAsync($"api/v1/tracking/workouts/{trackedWorkout.Id}",
				new UpdateTrackedWorkoutRequest(performedAt, duration))
			.ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		trackedWorkout = await httpClient.GetFromJsonAsync<GetTrackedWorkoutResponse>(trackedWorkoutUri).ConfigureAwait(false);

		await Assert.That(trackedWorkout).IsNotNull();
		await Assert.That(trackedWorkout!.WorkoutId).IsEqualTo(workout.Id);
		await Assert.That(trackedWorkout.PerformedAt - performedAt).IsLessThan(TimeSpan.FromSeconds(0.1));
		await Assert.That(trackedWorkout.Duration).IsEqualTo(duration);

		response = await httpClient.DeleteAsync(workoutUri).ConfigureAwait(false);

		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

		response = await httpClient.GetAsync(response.Headers.Location!).ConfigureAwait(false);
		await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
	}
}