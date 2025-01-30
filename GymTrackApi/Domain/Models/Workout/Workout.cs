using System.Diagnostics.CodeAnalysis;
using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Domain.Models.Tracking;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

[ValueObject<Guid>]
public readonly partial struct WorkoutId
{
	public static WorkoutId New() => From(Ulid.NewUlid().ToGuid());
}

[ValueObject<int>]
public readonly partial struct WorkoutExerciseIndex : IValueObject<int, WorkoutExerciseIndex>;

[ValueObject<int>]
public readonly partial struct WorkoutExerciseSetIndex : IValueObject<int, WorkoutExerciseSetIndex>;

public class Workout : IOwned
{
	public WorkoutId Id { get; private set; } = WorkoutId.New();

	public Name Name { get; private set; }

	public virtual List<Exercise> Exercises { get; private set; } = [];
	public virtual List<TrackedWorkout> TrackedWorkouts { get; private set; } = [];

	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	private Workout() { }

	private Workout(Name name, Owner owner)
	{
		Name = name;
		OwnerId = owner;
	}

	public static Workout CreatePublic(Name name) =>
		new(name, new Owner.Public());

	public static Workout CreateForUser(Name name, Guid userId) =>
		new(name, new Owner.User(userId));

	public void Update(Name name, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		Name = name;
	}

	public class Exercise : IIndexed<WorkoutExerciseIndex>, IDisplayOrdered
	{
		public WorkoutId WorkoutId { get; private set; }
		public WorkoutExerciseIndex Index { get; private set; }

		public virtual Workout Workout { get; private set; } = default!;

		public ExerciseInfoId ExerciseInfoId { get; private set; }
		public virtual ExerciseInfo.ExerciseInfo ExerciseInfo { get; private set; } = default!;

		public int DisplayOrder { get; set; }

		public virtual List<Set> Sets { get; private set; } = [];

		// ReSharper disable once UnusedMember.Local
		private Exercise() { }

		public Exercise(WorkoutId workoutId, WorkoutExerciseIndex index, ExerciseInfoId exerciseInfoId, int displayOrder)
		{
			WorkoutId = workoutId;
			Index = index;
			ExerciseInfoId = exerciseInfoId;
			DisplayOrder = displayOrder;
		}

		public class Set : IIndexed<WorkoutExerciseSetIndex>, IDisplayOrdered
		{
			public WorkoutId WorkoutId { get; private set; }
			public WorkoutExerciseIndex ExerciseIndex { get; private set; }
			public WorkoutExerciseSetIndex Index { get; private set; }

			public virtual Exercise Exercise { get; private set; } = default!;

			public ExerciseMetric Metric { get; private set; } = default!;

			public Reps Reps { get; private set; }

			public int DisplayOrder { get; set; }

			// ReSharper disable once UnusedMember.Local
			private Set() { }

			private Set(Exercise exercise, WorkoutExerciseSetIndex index, ExerciseMetric metric, Reps reps, int displayOrder)
			{
				WorkoutId = exercise.WorkoutId;
				ExerciseIndex = exercise.Index;
				Index = index;
				Exercise = exercise;
				Metric = metric;
				Reps = reps;
				DisplayOrder = displayOrder;
			}

			public static bool TryCreate(
				Exercise exercise,
				WorkoutExerciseSetIndex index,
				ExerciseMetric metric,
				Reps reps,
				int displayOrder,
				Guid userId,
				[NotNullWhen(true)] out Set? set,
				[NotNullWhen(false)] out ValidationError? error)
			{
				if (!exercise.Workout.CanBeModifiedBy(userId)) throw new PermissionError();

				var exerciseInfo = exercise.ExerciseInfo;
				if (!exerciseInfo.CanBeReadBy(userId)) throw new PermissionError();

				if (!exerciseInfo.AllowedMetricTypes.HasFlag(metric.Type))
				{
					error = new ValidationError("Invalid metric type.");
					set = null;
					return false;
				}

				set = new Set(exercise, index, metric, reps, displayOrder);
				error = null;
				return true;
			}

			public bool TryUpdate(
				ExerciseMetric metric,
				Reps reps,
				Guid userId,
				[NotNullWhen(false)] out ValidationError? error)
			{
				var exerciseInfo = Exercise.ExerciseInfo;
				if (!exerciseInfo.CanBeReadBy(userId)) throw new PermissionError();

				if (!exerciseInfo.AllowedMetricTypes.HasFlag(metric.Type))
				{
					error = new ValidationError("Invalid metric type.");
					return false;
				}

				Metric = metric;
				Reps = reps;

				error = null;
				return true;
			}
		}
	}
}