using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Domain.Common;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class Workout
{
	public static Workout CreateForEveryone(Name name) => new(name);

	public static Workout CreateForUser(Name name, ClaimsPrincipal user)
	{
		var workout = new Workout(name);
		var userWorkout = new UserWorkout(user.GetUserId(), workout.Id);
		workout.Users.Add(userWorkout);

		return workout;
	}

	public Id<Workout> Id { get; private set; } = Id<Workout>.New();

	public Name Name { get; private set; }

	public virtual List<UserWorkout> Users { get; private set; } = [];
	public virtual List<Exercise> Exercises { get; private set; } = [];

	private Workout(Name name) => Name = name;

	public class Exercise
	{
		public Id<Workout> WorkoutId { get; private set; }
		public int Index { get; private set; }

		public virtual Workout Workout { get; private set; } = default!;

		public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
		public virtual ExerciseInfo ExerciseInfo { get; private set; } = default!;

		public virtual List<Set> Sets { get; private set; } = [];

		// ReSharper disable once UnusedMember.Local
		private Exercise() { }

		public Exercise(Id<Workout> workoutId, int index, Id<ExerciseInfo> exerciseInfoId)
		{
			WorkoutId = workoutId;
			Index = index;
			ExerciseInfoId = exerciseInfoId;
		}

		public class Set
		{
			public Id<Workout> WorkoutId { get; private set; }
			public int ExerciseIndex { get; private set; }
			public int SetIndex { get; private set; }

			public virtual Exercise Exercise { get; private set; }

			public ExerciseMetric Metric { get; private set; }

			[Range(1, int.MaxValue, ErrorMessage = "Reps count can not be negative.")]
			public int Reps { get; private set; }

			private Set() { }

			public Set(Exercise exercise, int setIndex, ExerciseMetric metric, int reps)
			{
				WorkoutId = exercise.WorkoutId;
				ExerciseIndex = exercise.Index;
				SetIndex = setIndex;
				Exercise = exercise;
				Metric = metric;
				Reps = reps;
			}
		}
	}
}