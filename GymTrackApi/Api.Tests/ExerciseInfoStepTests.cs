﻿using Api.Dtos;
using Api.Routes.App.ExerciseInfos;
using Api.Routes.App.ExerciseInfos.Steps;
using Api.Tests.Mocks;
using Domain.Common;
using Domain.Models;
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
		new(Users.User1, 15, "ValidDescription", Placeholders.FormFile(), typeof(Created)),
		new(Users.User1, -1, "ValidDescription", null, typeof(ValidationProblem)),
		new(Users.User2, -2, "ValidDescription", Placeholders.FormFile(), typeof(ValidationProblem)),
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

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, int stepIndex, int accessedIndex, Type responseType)> GetExerciseInfoStepData() =>
	[
		new([Users.Admin1], Users.User1, 0, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.Admin1], Users.User1, 0, -1, typeof(ValidationProblem)),
		new([Users.User1], Users.Admin1, 0, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.Admin1, 0, 1, typeof(NotFound<string>)),
		new([Users.User2], Users.Admin1, 0, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.User1, 0, 0, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.User1, 1, 1, typeof(Ok<GetExerciseInfoStepResponse>)),
		new([Users.User1], Users.User1, 1, 2, typeof(NotFound<string>)),
		new([Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(GetExerciseInfoStepData))]
	public async Task GetExerciseInfoStep_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, int stepIndex, int accessedIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, owners)
			.Build()
			.ConfigureAwait(false);

		if (!Description.TryCreate("Test Description", out var description, out _)) throw new Exception("Invalid test case");

		exerciseInfo.Steps.Add(new ExerciseInfo.Step(exerciseInfo.Id, stepIndex, description, Option<FilePath>.None()));
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
}