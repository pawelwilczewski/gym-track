using Domain.Common;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;

namespace Api.Tests.Unit.Mocks;

internal static class MockDataContextBuilderExtensions
{
	public static MockDataContextBuilder WithWorkout(this MockDataContextBuilder builder, out Workout workout, IUserInfo owner)
	{
		workout = Workout.CreateForUser(Placeholders.RandomName(), owner.GetHttpContext().User.GetUserId());

		builder.WithEntity(workout);
		return builder;
	}

	public static MockDataContextBuilder WithExerciseInfo(this MockDataContextBuilder builder, out ExerciseInfo exerciseInfo, ExerciseMetricType allowedMetricTypes, IUserInfo owner)
	{
		exerciseInfo = ExerciseInfo.CreateForUser(
			Placeholders.RandomName(),
			Placeholders.RandomFilePath(),
			Placeholders.RandomDescription(),
			allowedMetricTypes,
			owner.GetHttpContext().User.GetUserId());

		builder.WithEntity(exerciseInfo);
		return builder;
	}

	public static MockDataContextBuilder WithAllUsers(this MockDataContextBuilder builder) => builder
		.WithUser(Users.Admin1)
		.WithUser(Users.User1)
		.WithUser(Users.User2);
}