using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.ExerciseInfo;

public class ExerciseInfo : IOwned
{
	public Id<ExerciseInfo> Id { get; } = Id<ExerciseInfo>.New();

	public Name Name { get; private set; }

	public FilePath? ThumbnailImage { get; private set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; private set; }

	public virtual List<Step> Steps { get; private set; } = [];
	public virtual List<Workout.Workout.Exercise> Exercises { get; private set; } = [];

	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	private ExerciseInfo() { }

	private ExerciseInfo(
		Id<ExerciseInfo> id,
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
		Id<ExerciseInfo>? id = null) =>
		new(id ?? Id<ExerciseInfo>.New(), name, thumbnailImage, description, allowedMetricTypes, new Owner.Public());

	public static ExerciseInfo CreateForUser(
		Name name,
		FilePath? thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		Guid userId,
		Id<ExerciseInfo>? id = null)
	{
		var exerciseInfo = new ExerciseInfo(
			id ?? Id<ExerciseInfo>.New(), name, thumbnailImage, description, allowedMetricTypes, new Owner.User(userId));
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

	public class Step : IIndexed, IDisplayOrdered
	{
		public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
		public int Index { get; private set; }

		public ExerciseInfo ExerciseInfo { get; private set; } = default!;

		public Description Description { get; private set; }
		public FilePath? ImageFile { get; private set; }

		public int DisplayOrder { get; set; }

		private Step() { }

		public Step(Id<ExerciseInfo> exerciseInfoId, int index, Description description, FilePath? imageFile, int displayOrder)
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
	}
}