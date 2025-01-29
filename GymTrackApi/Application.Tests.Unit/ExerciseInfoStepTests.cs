using Application.ExerciseInfo.Step.Commands;
using Application.ExerciseInfo.Step.DisplayOrder.Commands;
using Application.ExerciseInfo.Step.Dtos;
using Application.ExerciseInfo.Step.Queries;
using Application.Tests.Unit.Mocks;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Infrastructure.Persistence;
using OneOf.Types;

namespace Application.Tests.Unit;

internal sealed class ExerciseInfoStepTests
{
	public static IEnumerable<(IUserInfo user, Description description, Type responseType)>
		CreateExerciseInfoStepData() =>
	[
		(Users.Admin1, Description.From("ValidDescription"), typeof(Success<GetExerciseInfoStepResponse>)),
		(Users.User1, Description.From("ValidDescription"), typeof(Success<GetExerciseInfoStepResponse>))
	];

	[Test]
	[MethodDataSource(nameof(CreateExerciseInfoStepData))]
	public async Task CreateExerciseInfoStep_ReturnsCorrectResponse(
		IUserInfo user,
		Description description,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, user)
			.Build();

		var handler = new CreateExerciseInfoStepHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new CreateExerciseInfoStepCommand(
				exerciseInfo.Id,
				description,
				null,
				user.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, int accessedIndex, Type responseType)>
		GetExerciseInfoStepData() =>
	[
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.Admin1, Users.Admin1, 0, typeof(Success<GetExerciseInfoStepResponse>)),
		(Users.User1, Users.Admin1, 0, typeof(NotFound)),
		(Users.User2, Users.User1, 0, typeof(NotFound)),
		(Users.User1, Users.User1, 1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(GetExerciseInfoStepData))]
	public async Task GetExerciseInfoStep_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo accessor,
		int accessedIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owner)
			.Build();

		var step = new Domain.Models.ExerciseInfo.ExerciseInfo.Step(
			exerciseInfo.Id,
			0,
			Description.From("Test Description"),
			null,
			0);
		exerciseInfo.Steps.Add(step);
		await dataContext.SaveChangesAsync();

		var handler = new GetExerciseInfoStepHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetExerciseInfoStepQuery(exerciseInfo.Id, accessedIndex, accessor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	[Test]
	public async Task GetExerciseInfoStep_InvalidId_ReturnsNotFound()
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.Build();

		var handler = new GetExerciseInfoStepHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetExerciseInfoStepQuery(ExerciseInfoId.From(Guid.NewGuid()), 0, Users.User1.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(typeof(NotFound));
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo editor,
			Description description, bool replaceImage, Type responseType)>
		UpdateExerciseInfoStepData() =>
	[
		(Users.Admin1, Users.Admin1, Description.From("NewDesc"), false, typeof(Success)),
		(Users.User1, Users.User1, Description.From("Updated"), true, typeof(Success)),
		(Users.Admin1, Users.User1, Description.From("Invalid"), false, typeof(NotFound)),
		(Users.User1, Users.Admin1, Description.From("NoAccess"), false, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(UpdateExerciseInfoStepData))]
	public async Task UpdateExerciseInfoStep_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo editor,
		Description description,
		bool replaceImage,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owner)
			.Build();

		var originalStep = new Domain.Models.ExerciseInfo.ExerciseInfo.Step(
			exerciseInfo.Id,
			0,
			Description.From("Original"),
			null,
			0);
		exerciseInfo.Steps.Add(originalStep);
		await dataContext.SaveChangesAsync();

		var handler = new UpdateExerciseInfoStepHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new UpdateExerciseInfoStepCommand(
				exerciseInfo.Id,
				0,
				description,
				replaceImage,
				null,
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo editor,
		int displayOrder, Type responseType)> UpdateExerciseInfoStepDisplayOrderData() =>
	[
		(Users.Admin1, Users.Admin1, 4, typeof(Success)),
		(Users.User1, Users.User1, 2, typeof(Success)),
		(Users.Admin1, Users.User1, 6, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(UpdateExerciseInfoStepDisplayOrderData))]
	public async Task UpdateExerciseInfoStepDisplayOrder_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo editor,
		int displayOrder,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owner)
			.Build();

		var originalStep = new Domain.Models.ExerciseInfo.ExerciseInfo.Step(
			exerciseInfo.Id,
			0,
			Description.From("Original"),
			null,
			0);
		exerciseInfo.Steps.Add(originalStep);
		await dataContext.SaveChangesAsync();

		var handler = new UpdateExerciseInfoStepDisplayOrderHandler(
			new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new UpdateExerciseInfoStepDisplayOrderCommand(
				exerciseInfo.Id,
				0,
				displayOrder,
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo deleter,
		int deletedIndex, Type responseType)> DeleteExerciseInfoData() =>
	[
		(Users.Admin1, Users.Admin1, 0, typeof(Success)),
		(Users.User1, Users.User1, 0, typeof(Success)),
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.User1, Users.Admin1, 0, typeof(NotFound)),
		(Users.User1, Users.User1, 1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(DeleteExerciseInfoData))]
	public async Task DeleteExerciseInfoStep_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo deleter,
		int deletedIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owner)
			.Build();

		var originalStep = new Domain.Models.ExerciseInfo.ExerciseInfo.Step(
			exerciseInfo.Id,
			0,
			Description.From("Original"),
			null,
			0);
		exerciseInfo.Steps.Add(originalStep);
		await dataContext.SaveChangesAsync();

		var handler = new DeleteExerciseInfoStepHandler(
			new UserDataContextFactory(dataContext),
			new TempFileStoragePathProvider());

		var result = await handler.Handle(
			new DeleteExerciseInfoStepCommand(
				exerciseInfo.Id,
				deletedIndex,
				deleter.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}
}