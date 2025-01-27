using Api.Routes.App.ExerciseInfos;
using Api.Tests.Unit.Mocks;
using Domain.Models.ExerciseInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests.Unit;

internal sealed class ExerciseInfoTests
{
	public static IEnumerable<(IUserInfo user, string name, string description, ExerciseMetricType allowedMetricTypes, IFormFile? thumbnailImage, Type responseType)> CreateExerciseInfoData() =>
	[
		new(Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(Created)),
		new(Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(Created)),
		new(Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, null, typeof(Created)),
		new(Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance | ExerciseMetricType.Weight, Placeholders.FormFile(), typeof(Created)),
		new(Users.User1, "", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem)),
		new(Users.User2, ",.. -", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem)),
		new(Users.Admin1, "ValidName", null!, ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem)),
		new(Users.Admin1, null!, "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem)),
		new(Users.Admin1, "", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem)),
		new(Users.Admin1, "12345678901234567890123456789012345678901234567890", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(Created)),
		new(Users.Admin1, "12345678901234567890123456789012345678901234567890x", "ValidDescription", ExerciseMetricType.Distance, Placeholders.FormFile(), typeof(ValidationProblem))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoData))]
	public async Task CreateExerciseInfo_ReturnsCorrectResponse(IUserInfo user, string name, string description, ExerciseMetricType allowedMetricTypes, IFormFile? thumbnailImage, Type responseType)
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
				thumbnailImage,
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

	public static IEnumerable<EditExerciseTestData> EditExerciseInfoData() =>
	[
		new([Users.Admin1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance | ExerciseMetricType.Duration | ExerciseMetricType.Weight, false, null, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Duration | ExerciseMetricType.Weight, false, null, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "", "ValidDescription", ExerciseMetricType.Duration | ExerciseMetricType.Weight, false, null, typeof(ValidationProblem)),
		new([Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(NoContent)),
		new([Users.User1], Users.User1, "ValidName", "ValidDescription", 0, false, null, typeof(ValidationProblem)),
		new([Users.User1, Users.User2], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, false, null, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, true, Placeholders.FormFile(), typeof(NoContent)),
		new([Users.Admin1], Users.Admin1, "ValidName", "ValidDescription", ExerciseMetricType.Distance, true, null, typeof(NoContent))
	];

	[Test]
	[MethodDataSource(nameof(EditExerciseInfoData))]
	public async Task EditExerciseInfo_ReturnsCorrectResponse(EditExerciseTestData data)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, data.Owners)
			.Build()
			.ConfigureAwait(false);

		var result = await UpdateExerciseInfo.Handler(
				data.Editor.GetHttpContext(),
				exerciseInfo.Id.Value,
				data.Name,
				data.Description,
				data.AllowedMetricTypes,
				data.ReplaceThumbnail,
				data.Thumbnail,
				dataContext,
				new TempFileStoragePathProvider(),
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(data.ResponseType);
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

	internal readonly record struct EditExerciseTestData(
		IReadOnlyList<IUserInfo> Owners,
		IUserInfo Editor,
		string Name,
		string Description,
		ExerciseMetricType AllowedMetricTypes,
		bool ReplaceThumbnail,
		IFormFile? Thumbnail,
		Type ResponseType);
}