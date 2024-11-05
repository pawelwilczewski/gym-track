using System.Security.Claims;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;

namespace Api.Tests.Mocks;

internal static class MockDataContextBuilderExtensions
{
	private static Name GenerateRandomName()
	{
		Name.TryCreate($"Name_{Guid.NewGuid()}", out var name, out _);
		return name!;
	}

	private static Description GenerateRandomDescription()
	{
		Description.TryCreate($"Description_{Guid.NewGuid()}", out var description, out _);
		return description!;
	}

	private static FilePath GenerateRandomFilePath()
	{
		FilePath.TryCreate($"C:/Test/Files/File_{Guid.NewGuid()}", out var filePath, out _);
		return filePath!;
	}

	public static MockDataContextBuilder WithWorkout(this MockDataContextBuilder builder, out Workout workout, ClaimsPrincipal? user = null)
	{
		workout = user is null || user.IsInRole(Role.ADMINISTRATOR) ? Workout.CreateForEveryone(GenerateRandomName()) : Workout.CreateForUser(GenerateRandomName(), user);
		builder.WithEntity(workout);
		return builder;
	}

	public static MockDataContextBuilder WithExerciseInfo(this MockDataContextBuilder builder, out ExerciseInfo exerciseInfo, ExerciseMetricType allowedMetricTypes, ClaimsPrincipal? user = null)
	{
		exerciseInfo = user is null || user.IsInRole(Role.ADMINISTRATOR)
			? ExerciseInfo.CreateForEveryone(GenerateRandomName(), GenerateRandomFilePath(), GenerateRandomDescription(), allowedMetricTypes)
			: ExerciseInfo.CreateForUser(GenerateRandomName(), GenerateRandomFilePath(), GenerateRandomDescription(), allowedMetricTypes, user);
		builder.WithEntity(exerciseInfo);
		return builder;
	}
}