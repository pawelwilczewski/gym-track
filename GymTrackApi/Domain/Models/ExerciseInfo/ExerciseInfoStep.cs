using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.ExerciseInfo;

public class ExerciseInfoStep : IIndexed<ExerciseInfoStepIndex>, IDisplayOrdered
{
	public ExerciseInfoId ExerciseInfoId { get; private set; }
	public ExerciseInfoStepIndex Index { get; private set; }

	public ExerciseInfo ExerciseInfo { get; private set; } = default!;

	public Description Description { get; private set; }
	public FilePath? ImageFile { get; private set; }

	public int DisplayOrder { get; private set; }

	private ExerciseInfoStep() { }

	public ExerciseInfoStep(ExerciseInfoId exerciseInfoId, ExerciseInfoStepIndex index, Description description, FilePath? imageFile, int displayOrder)
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

[ValueObject<int>]
public readonly partial struct ExerciseInfoStepIndex : IValueObject<int, ExerciseInfoStepIndex>;