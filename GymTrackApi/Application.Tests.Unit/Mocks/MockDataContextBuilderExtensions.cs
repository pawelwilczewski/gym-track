using Domain.Models.ExerciseInfo;

namespace Application.Tests.Unit.Mocks;

internal static class MockDataContextBuilderExtensions
{
	public static MockDataContextBuilder WithWorkout(this MockDataContextBuilder builder, out Domain.Models.Workout.Workout workout, IUserInfo owner)
	{
		workout = Domain.Models.Workout.Workout.CreateForUser(Placeholders.RandomName(), owner.Id);

		builder.WithEntity(workout);
		return builder;
	}

	public static MockDataContextBuilder WithExerciseInfo(this MockDataContextBuilder builder, out Domain.Models.ExerciseInfo.ExerciseInfo exerciseInfo, ExerciseMetricType allowedMetricTypes, IUserInfo owner)
	{
		exerciseInfo = Domain.Models.ExerciseInfo.ExerciseInfo.CreateForUser(
			Placeholders.RandomName(),
			Placeholders.RandomFilePath(),
			Placeholders.RandomDescription(),
			allowedMetricTypes,
			owner.Id);

		builder.WithEntity(exerciseInfo);
		return builder;
	}

	public static MockDataContextBuilder WithAllUsers(this MockDataContextBuilder builder) => builder
		.WithUser(Users.Admin1)
		.WithUser(Users.User1)
		.WithUser(Users.User2);
}