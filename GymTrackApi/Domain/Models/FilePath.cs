using Domain.Validation;

namespace Domain.Models;

public sealed record class FilePath : ValidatedText<FilePath>
{
	public const int MAX_LENGTH = 500;

	protected override TextValidator Validator => TextValidators.FilePath;

	private FilePath(string value = "") : base(value) { }

	public override string ToString() => base.ToString();
}