using System.Security.Claims;
using Domain.Common;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class ExerciseInfo
{
	public Id<ExerciseInfo> Id { get; } = Id<ExerciseInfo>.New();

	public Name Name { get; private set; }

	public FilePath ThumbnailImage { get; set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; private set; }

	public virtual List<ExerciseStepInfo> Steps { get; private set; } = [];
	public virtual List<Exercise> Exercises { get; private set; } = [];
	public virtual List<UserExerciseInfo> Users { get; private set; } = [];

	private ExerciseInfo() { }

	private ExerciseInfo(Name name, FilePath thumbnailImage, Description description, ExerciseMetricType allowedMetricTypes)
	{
		Name = name;
		ThumbnailImage = thumbnailImage;
		Description = description;
		AllowedMetricTypes = allowedMetricTypes;
	}

	public static ExerciseInfo CreateForEveryone(
		Name name,
		FilePath thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes) =>
		new(name, thumbnailImage, description, allowedMetricTypes);

	public static ExerciseInfo CreateForUser(
		Name name,
		FilePath thumbnailImage,
		Description description,
		ExerciseMetricType allowedMetricTypes,
		ClaimsPrincipal user)
	{
		var exerciseInfo = new ExerciseInfo(name, thumbnailImage, description, allowedMetricTypes);
		var userExerciseInfo = new UserExerciseInfo(user.GetUserId(), exerciseInfo.Id);
		exerciseInfo.Users.Add(userExerciseInfo);
		return exerciseInfo;
	}
}

[Flags]
public enum ExerciseMetricType
{
	Weight = 1 << 0,
	Duration = 1 << 1,
	Distance = 1 << 2
}