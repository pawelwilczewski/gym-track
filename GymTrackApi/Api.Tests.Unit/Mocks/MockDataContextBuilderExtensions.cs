using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;

namespace Api.Tests.Unit.Mocks;

internal static class MockDataContextBuilderExtensions
{
	public static MockDataContextBuilder WithWorkout(this MockDataContextBuilder builder, out Workout workout, IReadOnlyList<IUserInfo> owners)
	{
		switch (owners)
		{
			case []:
			case [AdminInfo]:
			{
				workout = Workout.CreatePublic(Placeholders.RandomName());
				break;
			}
			default:
			{
				if (owners.Any(owner => owner is AdminInfo))
				{
					return WithWorkout(builder, out workout, owners.Where(owner => owner is not AdminInfo).ToList());
				}

				workout = Workout.CreateForUser(Placeholders.RandomName(), owners[0].GetHttpContext().User);
				for (var i = 1; i < owners.Count; ++i)
				{
					workout.Users.Add(new UserWorkout(owners[i].Id, workout.Id));
				}

				break;
			}
		}

		builder.WithEntity(workout);
		return builder;
	}

	public static MockDataContextBuilder WithExerciseInfo(this MockDataContextBuilder builder, out ExerciseInfo exerciseInfo, ExerciseMetricType allowedMetricTypes, IReadOnlyList<IUserInfo> owners)
	{
		switch (owners)
		{
			case []:
			case [AdminInfo]:
			{
				exerciseInfo = ExerciseInfo.CreatePublic(Placeholders.RandomName(), Placeholders.RandomFilePath(), Placeholders.RandomDescription(), allowedMetricTypes);
				break;
			}
			default:
			{
				if (owners.Any(owner => owner is AdminInfo))
				{
					return WithExerciseInfo(builder, out exerciseInfo, allowedMetricTypes, owners.Where(owner => owner is not AdminInfo).ToList());
				}

				exerciseInfo = ExerciseInfo.CreateForUser(Placeholders.RandomName(), Placeholders.RandomFilePath(), Placeholders.RandomDescription(), allowedMetricTypes, owners[0].GetHttpContext().User);
				for (var i = 1; i < owners.Count; ++i)
				{
					exerciseInfo.Users.Add(new UserExerciseInfo(owners[i].Id, exerciseInfo.Id));
				}

				break;
			}
		}

		builder.WithEntity(exerciseInfo);
		return builder;
	}

	public static MockDataContextBuilder WithAllUsers(this MockDataContextBuilder builder) => builder
		.WithUser(Users.Admin1)
		.WithUser(Users.User1)
		.WithUser(Users.User2);
}