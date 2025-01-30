using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Domain.Models.Workout;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.ExerciseInfo;

public class ExerciseInfo : IOwned
{
	public ExerciseInfoId Id { get; } = ExerciseInfoId.New();

	public Name Name { get; private set; }

	public FilePath? ThumbnailImage { get; private set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; private set; }

	public virtual List<WorkoutExercise> Exercises { get; private set; } = [];

	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	public IReadOnlyList<ExerciseInfoStep> Steps => steps.AsReadOnly();
	private readonly List<ExerciseInfoStep> steps = [];

	private ExerciseInfo() { }

	private ExerciseInfo(
		ExerciseInfoId id,
		Name name,
		FilePath? thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		Owner owner)
	{
		Id = id;
		Name = name;
		ThumbnailImage = thumbnailImage;
		Description = description;
		AllowedMetricTypes = allowedMetricTypes;
		OwnerId = owner;
	}

	public static ExerciseInfo CreatePublic(
		Name name,
		FilePath? thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		ExerciseInfoId? id = null) =>
		new(id ?? ExerciseInfoId.New(), name, thumbnailImage, description, allowedMetricTypes, new Owner.Public());

	public static ExerciseInfo CreateForUser(
		Name name,
		FilePath? thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		Guid userId,
		ExerciseInfoId? id = null)
	{
		var exerciseInfo = new ExerciseInfo(
			id ?? ExerciseInfoId.New(), name, thumbnailImage, description, allowedMetricTypes, new Owner.User(userId));
		return exerciseInfo;
	}

	public void Update(Name name, Description description, FilePath? thumbnailImage, ExerciseMetricType allowedMetricTypes, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		Name = name;
		Description = description;
		ThumbnailImage = thumbnailImage;
		AllowedMetricTypes = allowedMetricTypes;
	}

	public void UpdateThumbnailImage(FilePath? thumbnailImage, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		ThumbnailImage = thumbnailImage;
	}

	public void AddStep(ExerciseInfoStep step, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		steps.Add(step);
	}

	public void RemoveStep(ExerciseInfoStep step, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		steps.Remove(step);
	}
}

[ValueObject<Guid>]
public readonly partial struct ExerciseInfoId
{
	public static ExerciseInfoId New() => From(Ulid.NewUlid().ToGuid());
}