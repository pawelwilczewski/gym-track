using Api.Dtos;
using Api.Routes.App.ExerciseInfos.Steps;
using Api.Tests.Unit.Mocks;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests.Unit;

internal sealed class ExerciseInfoStepTests
{
	public static IEnumerable<(IUserInfo user, string description, IFormFile? imageFile, Type responseType)> CreateExerciseInfoStepData() =>
	[
		new(Users.Admin1, "ValidDescription", null, typeof(Created)),
		new(Users.User1, "ValidDescription", null, typeof(Created)),
		new(Users.User1, "ValidDescription", Placeholders.FormFile(), typeof(Created)),
		new(Users.Admin1, null!, null, typeof(ValidationProblem)),
		new(Users.Admin1, "ValidDescription", null, typeof(Created))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoStepData))]
	public async Task CreateExerciseInfoStep_ReturnsCorrectResponse(IUserInfo user, string description, IFormFile? imageFile, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, [user])
			.Build()
			.ConfigureAwait(false);

		var result = await CreateExerciseInfoStep.Handler(
				user.GetHttpContext(),
				exerciseInfo.Id.Value,
				description,
				imageFile,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, int accessedIndex, Type responseType)> GetExerciseInfoStepData() =>
	[
		new([Users.Admin1], Users.User1, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.Admin1], Users.User1, -1, typeof(NotFound<string>)),
		new([Users.User1], Users.Admin1, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.Admin1, 1, typeof(NotFound<string>)),
		new([Users.User2], Users.Admin1, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.User1, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.User1, 1, typeof(NotFound<string>)),
		new([Users.User1], Users.User1, 2, typeof(NotFound<string>)),
		new([Users.User2], Users.User1, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(GetExerciseInfoStepData))]
	public async Task GetExerciseInfoStep_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, int accessedIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		if (!Description.TryCreate("Test Description", out var description, out _)) throw new Exception("Invalid test case");

		exerciseInfo.Steps.Add(new ExerciseInfo.Step(exerciseInfo.Id, 0, description, null, 0));
		await dataContext.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);

		var result = await GetExerciseInfoStep.Handler(
				accessor.GetHttpContext(),
				exerciseInfo.Id.Value,
				accessedIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	public async Task GetExerciseInfoStep_InvalidGuid_ReturnsNotFound()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out _, ExerciseMetricType.Distance, [Users.User1])
			.Build()
			.ConfigureAwait(false);

		var result = await GetExerciseInfoStep.Handler(
				Users.User1.GetHttpContext(),
				Guid.NewGuid(),
				0,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(NotFound<string>));
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string description, bool replaceImage, IFormFile? image, Type responseType)> EditExerciseInfoStepData() =>
	[
		new([Users.Admin1], Users.User1, "ValidDescription", false, null, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, "ValidDescription", false, null, typeof(NoContent)),
		new([Users.Admin1], Users.Admin1, "ValidDescription", true, null, typeof(NoContent)),
		new([Users.Admin1], Users.Admin1, "ValidDescription", true, Placeholders.FormFile(), typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "ValidDescription", false, null, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "ValidDescription", false, null, typeof(NoContent)),
		new([Users.User1], Users.User1, null!, false, null, typeof(ValidationProblem)),
		new([Users.User1], Users.User1, Placeholders.RandomStringNCharacters(Description.MAX_LENGTH + 1), false, null, typeof(ValidationProblem)),
		new([Users.User1, Users.User2], Users.User1, "ValidDescription", false, null, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, "ValidDescription", false, null, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidDescription", false, null, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidDescription", false, null, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(EditExerciseInfoStepData))]
	public async Task EditExerciseInfoStep_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string description, bool replaceImage, IFormFile? image, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		if (!Description.TryCreate("Test Description", out var originalDescription, out _)) throw new Exception("Invalid test case");

		var originalStep = new ExerciseInfo.Step(exerciseInfo.Id, 0, originalDescription, null, 0);
		exerciseInfo.Steps.Add(originalStep);
		await dataContext.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);

		var result = await EditExerciseInfoStep.Handler(
				editor.GetHttpContext(),
				exerciseInfo.Id.Value,
				0,
				description,
				replaceImage,
				image,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, int deletedIndex, Type responseType)> DeleteExerciseInfoData() =>
	[
		new([Users.Admin1], Users.User1, 0, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, 0, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, 0, typeof(NoContent)),
		new([Users.User1], Users.User1, 0, typeof(NoContent)),
		new([Users.User1], Users.User1, 1, typeof(NotFound<string>)),
		new([Users.User1, Users.User2], Users.User1, 0, typeof(ForbidHttpResult)),
		new([Users.User1, Users.User2], Users.User1, 1, typeof(NotFound<string>)),
		new([Users.User2], Users.User1, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(DeleteExerciseInfoData))]
	public async Task DeleteExerciseInfoStep_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, int deletedIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		if (!Description.TryCreate("Test Description", out var originalDescription, out _)) throw new Exception("Invalid test case");

		var originalStep = new ExerciseInfo.Step(exerciseInfo.Id, 0, originalDescription, null, 0);
		exerciseInfo.Steps.Add(originalStep);
		await dataContext.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);

		var result = await DeleteExerciseInfoStep.Handler(
				deleter.GetHttpContext(),
				exerciseInfo.Id.Value,
				deletedIndex,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}