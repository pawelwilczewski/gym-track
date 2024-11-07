using Api.Dtos;
using Api.Routes.App.ExerciseInfos;
using Api.Routes.App.ExerciseInfos.Steps;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class ExerciseInfoStepTests
{
	public static IEnumerable<(IUserInfo user, int index, string description, IFormFile? imageFile, Type responseType)> CreateExerciseInfoStepData() =>
	[
		new(Users.Admin1, 0, "ValidDescription", null, typeof(Created)),
		new(Users.User1, 1, "ValidDescription", null, typeof(Created)),
		new(Users.User1, 15, "ValidDescription", FakeData.FormFile(), typeof(Created)),
		new(Users.User1, -1, "ValidDescription", null, typeof(ValidationProblem)),
		new(Users.User2, -2, "ValidDescription", FakeData.FormFile(), typeof(ValidationProblem)),
		new(Users.Admin1, 0, null!, null, typeof(ValidationProblem)),
		new(Users.Admin1, -1, "ValidDescription", null, typeof(ValidationProblem)),
		new(Users.Admin1, 5, "ValidDescription", null, typeof(Created))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoStepData))]
	public async Task CreateExerciseInfoStep_ReturnsCorrectResponse(IUserInfo user, int index, string description, IFormFile? imageFile, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, [user])
			.Build()
			.ConfigureAwait(false);

		var result = await CreateExerciseInfoStep.Handler(
				user.GetHttpContext(),
				exerciseInfo.Id.Value,
				index,
				description,
				imageFile,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}