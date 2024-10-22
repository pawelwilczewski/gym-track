using Domain.Common;
using Domain.Validation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public sealed record class FilePath : ValidatedText<FilePath>
{
	public const int MAX_LENGTH = 500;

	public static ValueConverter<Option<FilePath>, string> OptionalConverter { get; } = new(
		option => option.Map(filePath => filePath.ToString()).Reduce(null)!,
		value => string.IsNullOrWhiteSpace(value)
			? Option<FilePath>.None()
			: Option<FilePath>.Some(new FilePath(value)));

	public static ValueComparer<Option<FilePath>> OptionalComparer { get; } = new(
		(a, b) => (a == null && b == null) || (a != null && b != null && a == b),
		value => value.GetHashCode(),
		text => text.Reduce(null) == null
			? Option<FilePath>.None()
			: Option<FilePath>.Some(new FilePath(text.Reduce(null)!)));

	protected override TextValidator Validator => TextValidators.FilePath;

	private FilePath(string value = "") : base(value) { }

	public override string ToString() => base.ToString();
}