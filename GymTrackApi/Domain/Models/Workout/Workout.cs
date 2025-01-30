using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Domain.Models.Tracking;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class Workout : IOwned
{
	public WorkoutId Id { get; private set; } = WorkoutId.New();

	public Name Name { get; private set; }

	public virtual IReadOnlyList<TrackedWorkout> TrackedWorkouts { get; private set; } = [];

	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	public IReadOnlyList<WorkoutExercise> Exercises => exercises.AsReadOnly();
	private readonly List<WorkoutExercise> exercises = [];

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

	public void AddExercise(WorkoutExercise exercise, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		exercises.Add(exercise);
	}

	public void RemoveExercise(WorkoutExercise exercise, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		exercises.Remove(exercise);
	}
}

[ValueObject<Guid>]
public readonly partial struct WorkoutId
{
	public static WorkoutId New() => From(Ulid.NewUlid().ToGuid());
}