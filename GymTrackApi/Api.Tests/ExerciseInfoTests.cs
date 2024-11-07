using Api.Routes.App.ExerciseInfos;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class ExerciseInfoTests
{
	public static IEnumerable<(IUserInfo user, string name, string description, ExerciseMetricType allowedMetricTypes, Type responseType)> CreateExerciseInfoData() =>
	[
		new(Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(Created)),
		new(Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(Created)),
		new(Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance | ExerciseMetricType.Weight, typeof(Created)),
		new(Users.User1, "", "ValidDescription", ExerciseMetricType.Distance, typeof(ValidationProblem)),
		new(Users.User2, ",.. -", "ValidDescription", ExerciseMetricType.Distance, typeof(ValidationProblem)),
		new(Users.Admin1, "ValidName", null!, ExerciseMetricType.Distance, typeof(ValidationProblem)),
		new(Users.Admin1, null!, "ValidDescription", ExerciseMetricType.Distance, typeof(ValidationProblem)),
		new(Users.Admin1, "", "ValidDescription", ExerciseMetricType.Distance, typeof(ValidationProblem)),
		new(Users.Admin1, "12345678901234567890123456789012345678901234567890", "ValidDescription", ExerciseMetricType.Distance, typeof(Created)),
		new(Users.Admin1, "12345678901234567890123456789012345678901234567890x", "ValidDescription", ExerciseMetricType.Distance, typeof(ValidationProblem))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoData))]
	public async Task CreateExerciseInfo_ReturnsCorrectResponse(IUserInfo user, string name, string description, ExerciseMetricType allowedMetricTypes, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.Build()
			.ConfigureAwait(false);

		var result = await CreateExerciseInfo.Handler(
				user.GetHttpContext(),
				name,
				description,
				allowedMetricTypes,
				RandomGenerator.FormFile(),
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}