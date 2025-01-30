using Application.ExerciseInfo.Commands;
using Application.ExerciseInfo.Dtos;
using Application.ExerciseInfo.Queries;
using Application.Tests.Unit.Mocks;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Infrastructure.Persistence;
using OneOf.Types;

namespace Application.Tests.Unit;

internal sealed class ExerciseInfoTests
{
	public static IEnumerable<(IUserInfo user, Name name, Description description,
		SomeExerciseMetricTypes allowedMetricTypes, Type responseType)> CreateExerciseInfoData() =>
	[
		(Users.Admin1, Name.From("ValidName"), Description.From("ValidDesc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(Success<GetExerciseInfoResponse>)),
		(Users.User1, Name.From("ValidName"), Description.From("ValidDesc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(Success<GetExerciseInfoResponse>)),
		(Users.User1, Name.From("dsad"), Description.From("ValidDesc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(Success<GetExerciseInfoResponse>)),
		(Users.User1, Name.From("LongName1234567890123456789012345678901234"),
			Description.From("ValidDesc"), SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(Success<GetExerciseInfoResponse>)),
		(Users.Admin1, Name.From("ValidName"), Description.From("Valid"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(Success<GetExerciseInfoResponse>))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoData))]
	public async Task CreateExerciseInfo_ReturnsCorrectResponse(
		IUserInfo user,
		Name name,
		Description description,
		SomeExerciseMetricTypes allowedMetricTypes,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.Build();

		var handler = new CreateExerciseInfoHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new CreateExerciseInfoCommand(
				name,
				description,
				null,
				allowedMetricTypes,
				user.Id),
			CancellationToken.None);

		await Assert.That(result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, Type responseType)>
		GetExerciseInfoData() =>
	[
		(Users.Admin1, Users.User1, typeof(NotFound)),
		(Users.User1, Users.User1, typeof(Success<GetExerciseInfoResponse>)),
		(Users.User1, Users.Admin1, typeof(NotFound)),
		(Users.User2, Users.User1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(GetExerciseInfoData))]
	public async Task GetExerciseInfo_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo accessor,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), owner)
			.Build();

		var handler = new GetExerciseInfoHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetExerciseInfoQuery(exerciseInfo.Id, accessor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	[Test]
	public async Task GetExerciseInfo_InvalidId_ReturnsNotFound()
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.Build();

		var handler = new GetExerciseInfoHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetExerciseInfoQuery(ExerciseInfoId.From(Guid.NewGuid()), Users.User1.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(typeof(NotFound));
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo editor,
			Name name, Description description, SomeExerciseMetricTypes metricTypes, Type responseType)>
		UpdateExerciseInfoData() =>
	[
		(Users.Admin1, Users.Admin1, Name.From("NewName"), Description.From("NewDesc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Duration), typeof(Success)),
		(Users.User1, Users.User1, Name.From("Valid"), Description.From(""),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Weight), typeof(Success)),
		(Users.Admin1, Users.User1, Name.From("Valid"), Description.From("Desc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(NotFound)),
		(Users.User1, Users.Admin1, Name.From("Invalid"), Description.From("Desc"),
			SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(UpdateExerciseInfoData))]
	public async Task UpdateExerciseInfo_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo editor,
		Name name,
		Description description,
		SomeExerciseMetricTypes metricTypes,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), owner)
			.Build();

		var handler = new UpdateExerciseInfoHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new UpdateExerciseInfoCommand(
				exerciseInfo.Id,
				name,
				description,
				false,
				null,
				metricTypes,
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo deleter, Type responseType)>
		DeleteExerciseInfoData() =>
	[
		(Users.Admin1, Users.Admin1, typeof(Success)),
		(Users.User1, Users.User1, typeof(Success)),
		(Users.Admin1, Users.User1, typeof(NotFound)),
		(Users.User1, Users.Admin1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(DeleteExerciseInfoData))]
	public async Task DeleteExerciseInfo_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo deleter,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, SomeExerciseMetricTypes.From(ExerciseMetricType.Distance), owner)
			.Build();

		var handler = new DeleteExerciseInfoHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new DeleteExerciseInfoCommand(exerciseInfo.Id, deleter.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}
}