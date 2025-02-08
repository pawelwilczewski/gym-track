using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Domain.Models.User;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class WorkoutExercise : IIndexed<WorkoutExerciseIndex>, IDisplayOrdered
{
	public WorkoutId WorkoutId { get; private set; }
	public WorkoutExerciseIndex Index { get; private set; }

	public virtual Workout Workout { get; private set; } = default!;

	public ExerciseInfoId ExerciseInfoId { get; private set; }
	public virtual ExerciseInfo.ExerciseInfo ExerciseInfo { get; private set; } = default!;

	public int DisplayOrder { get; private set; }

	public IReadOnlyList<WorkoutExerciseSet> Sets => sets.AsReadOnly();
	private readonly List<WorkoutExerciseSet> sets = [];

	// ReSharper disable once UnusedMember.Local
	private WorkoutExercise() { }

	public WorkoutExercise(
		WorkoutId workoutId,
		WorkoutExerciseIndex index,
		ExerciseInfoId exerciseInfoId,
		int displayOrder,
		Workout workout)
	{
		WorkoutId = workoutId;
		Index = index;
		ExerciseInfoId = exerciseInfoId;
		DisplayOrder = displayOrder;
		Workout = workout;
	}

	public void AddSet(WorkoutExerciseSet set, UserId userId)
	{
		if (!Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		sets.Add(set);
	}

	public void RemoveSet(WorkoutExerciseSet set, UserId userId)
	{
		if (!Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		sets.Remove(set);
	}

	public void UpdateDisplayOrder(int displayOrder, UserId userId)
	{
		if (!Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		DisplayOrder = displayOrder;
	}
}

[ValueObject<int>]
public readonly partial struct WorkoutExerciseIndex : IValueObject<int, WorkoutExerciseIndex>;