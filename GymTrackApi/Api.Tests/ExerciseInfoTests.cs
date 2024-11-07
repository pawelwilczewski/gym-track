using Api.Dtos;
using Api.Routes.App.ExerciseInfos;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http;
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
				Placeholders.FormFile(),
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, Type responseType)> GetExerciseInfoData() =>
	[
		new([Users.Admin1], Users.User1, typeof(Ok<GetExerciseInfoResponse>)),
		new([Users.User1], Users.Admin1, typeof(Ok<GetExerciseInfoResponse>)),
		new([Users.User2], Users.Admin1, typeof(Ok<GetExerciseInfoResponse>)),
		new([Users.User1], Users.User1, typeof(Ok<GetExerciseInfoResponse>)),
		new([Users.User2], Users.User1, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(GetExerciseInfoData))]
	public async Task GetExerciseInfo_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		var result = await GetExerciseInfo.Handler(
				accessor.GetHttpContext(),
				exerciseInfo.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	public async Task GetExerciseInfo_InvalidGuid_ReturnsNotFound()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out _, ExerciseMetricType.Distance, [Users.User1])
			.Build()
			.ConfigureAwait(false);

		var result = await GetExerciseInfo.Handler(
				Users.User1.GetHttpContext(),
				Guid.NewGuid(),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(NotFound<string>));
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string name, string description, ExerciseMetricType allowedMetricTypes, Type responseType)> EditExerciseInfoData() =>
	[
		new([Users.Admin1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance | ExerciseMetricType.Duration | ExerciseMetricType.Weight, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Duration | ExerciseMetricType.Weight, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "", "ValidDescription", ExerciseMetricType.Duration | ExerciseMetricType.Weight, typeof(ValidationProblem)),
		new([Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(NoContent)),
		new([Users.User1], Users.User1, "ValidName", "ValidDescription", 0, typeof(ValidationProblem)),
		new([Users.User1, Users.User2], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(EditExerciseInfoData))]
	public async Task EditExerciseInfo_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string name, string description, ExerciseMetricType allowedMetricTypes, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		var result = await EditExerciseInfo.Handler(
				editor.GetHttpContext(),
				exerciseInfo.Id.Value,
				new EditExerciseInfoRequest(name, description, allowedMetricTypes),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, IFormFile thumbnail, Type responseType)> EditExerciseInfoThumbnailData() =>
	[
		new([Users.Admin1], Users.User1, Placeholders.FormFile(), typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, Placeholders.FormFile(), typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, Placeholders.FormFile(), typeof(NoContent)),
		new([Users.User1], Users.User1, Placeholders.FormFile(), typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, Placeholders.FormFile(), typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, Placeholders.FormFile(), typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, Placeholders.FormFile(), typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, Placeholders.FormFile(), typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(EditExerciseInfoThumbnailData))]
	public async Task EditExerciseInfoThumbnail_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, IFormFile thumbnail, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		var result = await EditExerciseInfoThumbnail.Handler(
				editor.GetHttpContext(),
				exerciseInfo.Id.Value,
				thumbnail,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, Type responseType)> DeleteExerciseInfoData() =>
	[
		new([Users.Admin1], Users.User1, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, typeof(NoContent)),
		new([Users.User1], Users.User1, typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(DeleteExerciseInfoData))]
	public async Task DeleteExerciseInfo_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		var result = await DeleteExerciseInfo.Handler(
				deleter.GetHttpContext(),
				exerciseInfo.Id.Value,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}