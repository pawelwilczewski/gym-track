using System.Diagnostics.CodeAnalysis;
using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class WorkoutExerciseSet : IIndexed<WorkoutExerciseSetIndex>, IDisplayOrdered
{
	public WorkoutId WorkoutId { get; private set; }
	public WorkoutExerciseIndex ExerciseIndex { get; private set; }
	public WorkoutExerciseSetIndex Index { get; private set; }

	public virtual WorkoutExercise Exercise { get; private set; } = default!;

	public ExerciseMetric Metric { get; private set; } = default!;

	public Reps Reps { get; private set; }

	public int DisplayOrder { get; private set; }

	// ReSharper disable once UnusedMember.Local
	private WorkoutExerciseSet() { }

	private WorkoutExerciseSet(WorkoutExercise exercise, WorkoutExerciseSetIndex index, ExerciseMetric metric, Reps reps, int displayOrder)
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
		WorkoutExercise exercise,
		WorkoutExerciseSetIndex index,
		ExerciseMetric metric,
		Reps reps,
		int displayOrder,
		Guid userId,
		[NotNullWhen(true)] out WorkoutExerciseSet? set,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (!exercise.Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		var exerciseInfo = exercise.ExerciseInfo;
		if (!exerciseInfo.CanBeReadBy(userId)) throw new PermissionError();

		if (!exerciseInfo.AllowedMetricTypes.Value.HasFlag(metric.Type.Value))
		{
			error = new ValidationError("Invalid metric type.");
			set = null;
			return false;
		}

		set = new WorkoutExerciseSet(exercise, index, metric, reps, displayOrder);
		error = null;
		return true;
	}

	public bool TryUpdate(
		ExerciseMetric metric,
		Reps reps,
		Guid userId,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (!Exercise.Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		var exerciseInfo = Exercise.ExerciseInfo;
		if (!exerciseInfo.CanBeReadBy(userId)) throw new PermissionError();

		if (!exerciseInfo.AllowedMetricTypes.Value.HasFlag(metric.Type.Value))
		{
			error = new ValidationError("Invalid metric type.");
			return false;
		}

		Metric = metric;
		Reps = reps;

		error = null;
		return true;
	}

	public void UpdateDisplayOrder(int displayOrder, Guid userId)
	{
		if (!Exercise.Workout.CanBeModifiedBy(userId)) throw new PermissionError();

		DisplayOrder = displayOrder;
	}
}

[ValueObject<int>]
public readonly partial struct WorkoutExerciseSetIndex : IValueObject<int, WorkoutExerciseSetIndex>;