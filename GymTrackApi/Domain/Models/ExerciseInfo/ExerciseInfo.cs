using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.ExerciseInfo;

[ValueObject<Guid>]
public readonly partial struct ExerciseInfoId
{
	public static ExerciseInfoId New() => From(Ulid.NewUlid().ToGuid());
}

[ValueObject<int>]
public readonly partial struct ExerciseInfoStepIndex : IValueObject<int, ExerciseInfoStepIndex>;

public class ExerciseInfo : IOwned
{
	public ExerciseInfoId Id { get; } = ExerciseInfoId.New();

	public Name Name { get; private set; }

	public FilePath? ThumbnailImage { get; private set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; private set; }

	public virtual List<Workout.Workout.Exercise> Exercises { get; private set; } = [];

	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	public IReadOnlyList<Step> Steps => steps.AsReadOnly();
	private readonly List<Step> steps = [];

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

	public void AddStep(Step step, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		steps.Add(step);
	}

	public void RemoveStep(Step step, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		steps.Remove(step);
	}

	public class Step : IIndexed<ExerciseInfoStepIndex>, IDisplayOrdered
	{
		public ExerciseInfoId ExerciseInfoId { get; private set; }
		public ExerciseInfoStepIndex Index { get; private set; }

		public ExerciseInfo ExerciseInfo { get; private set; } = default!;

		public Description Description { get; private set; }
		public FilePath? ImageFile { get; private set; }

		public int DisplayOrder { get; private set; }

		private Step() { }

		public Step(ExerciseInfoId exerciseInfoId, ExerciseInfoStepIndex index, Description description, FilePath? imageFile, int displayOrder)
		{
			ExerciseInfoId = exerciseInfoId;
			Index = index;
			Description = description;
			ImageFile = imageFile;
			DisplayOrder = displayOrder;
		}

		public void Update(Description description, FilePath? imageFile, Guid userId)
		{
			if (!ExerciseInfo.CanBeModifiedBy(userId)) throw new PermissionError();

			Description = description;
			ImageFile = imageFile;
		}

		public void UpdateDisplayOrder(int displayOrder, Guid userId)
		{
			if (!ExerciseInfo.CanBeModifiedBy(userId)) throw new PermissionError();

			DisplayOrder = displayOrder;
		}
	}
}