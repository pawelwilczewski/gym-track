using System.Security.Claims;
using Domain.Common;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class ExerciseInfo
{
	public Id<ExerciseInfo> Id { get; } = Id<ExerciseInfo>.New();

	public Name Name { get; private set; }

	public FilePath ThumbnailImage { get; private set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; set; }

	public virtual List<Step> Steps { get; private set; } = [];
	public virtual List<Workout.Exercise> Exercises { get; private set; } = [];
	public virtual List<UserExerciseInfo> Users { get; private set; } = [];

	private ExerciseInfo() { }

	private ExerciseInfo(
		Id<ExerciseInfo> id,
		Name name,
		FilePath thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes)
	{
		Id = id;
		Name = name;
		ThumbnailImage = thumbnailImage;
		Description = description;
		AllowedMetricTypes = allowedMetricTypes;
	}

	public static ExerciseInfo CreateForEveryone(
		Name name,
		FilePath thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		Id<ExerciseInfo>? id = null) =>
		new(id ?? Id<ExerciseInfo>.New(), name, thumbnailImage, description, allowedMetricTypes);

	public static ExerciseInfo CreateForUser(
		Name name,
		FilePath thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		ClaimsPrincipal user,
		Id<ExerciseInfo>? id = null)
	{
		var exerciseInfo = new ExerciseInfo(
			id ?? Id<ExerciseInfo>.New(), name, thumbnailImage, description, allowedMetricTypes);
		var userExerciseInfo = new UserExerciseInfo(user.GetUserId(), exerciseInfo.Id);
		exerciseInfo.Users.Add(userExerciseInfo);
		return exerciseInfo;
	}

	public class Step
	{
		public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
		public Index Index { get; private set; }

		public ExerciseInfo ExerciseInfo { get; private set; } = default!;

		public Description Description { get; private set; }
		public FilePath? ImageFile { get; set; }

		private Step() { }

		public Step(Id<ExerciseInfo> exerciseInfoId, Index index, Description description, FilePath? imageFile)
		{
			ExerciseInfoId = exerciseInfoId;
			Index = index;
			Description = description;
			ImageFile = imageFile;
		}
	}
}