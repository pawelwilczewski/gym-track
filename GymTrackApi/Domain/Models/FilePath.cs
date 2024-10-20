using Domain.Validation;

namespace Domain.Models;

public sealed record class FilePath() : ValidatedText<FilePath>(string.Empty)
{
	public const int MAX_LENGTH = 500;

	protected override TextValidator Validator => TextValidators.FilePath;

	public static TextValidationResult TryCreate(string value, out FilePath filePath)
	{
		filePath = new FilePath();
		return filePath.Set(value);
	}

	public override string ToString() => base.ToString();
}

public sealed record class OptionalFilePath() : ValidatedText<OptionalFilePath>(string.Empty)
{
	protected override TextValidator Validator => TextValidators.OptionalFilePath;

	public static TextValidationResult TryCreate(string value, out OptionalFilePath optionalFilePath)
	{
		optionalFilePath = new OptionalFilePath();
		return optionalFilePath.Set(value);
	}

	public override string ToString() => base.ToString();
}