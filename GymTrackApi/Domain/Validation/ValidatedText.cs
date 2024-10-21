using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Validation;

public abstract record class ValidatedText<T> where T : ValidatedText<T>
{
	public static ValueConverter<T, string> Converter { get; } = new(
		text => text.Value,
		value => (T)Activator.CreateInstance(
			typeof(T),
			BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
			null,
			new object[] { value },
			null)!);

	public static ValueComparer<T> Comparer { get; } = new(
		(a, b) => (a == null && b == null) || (a != null && b != null && a.Value == b.Value),
		value => value.GetHashCode(),
		text => (T)Activator.CreateInstance(
			typeof(T),
			BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
			null,
			new object[] { text },
			null)!);

	public static bool TryCreate(
		string value,
		[NotNullWhen(true)] out T? text,
		[NotNullWhen(false)] out TextValidationResult.Invalid? invalid)
	{
		text = (T)Activator.CreateInstance(
			typeof(T),
			BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
			null,
			[""],
			null)!;

		var result = text.Set(value);
		if (result is TextValidationResult.Success)
		{
			invalid = null;
			return true;
		}

		invalid = (TextValidationResult.Invalid)result;
		return false;
	}

	private string Value { get; set; }
	protected abstract TextValidator Validator { get; }

	protected ValidatedText(string value = "") => Value = value;

	public TextValidationResult Set(string value)
	{
		switch (Validator(value))
		{
			case TextValidationResult.Invalid invalid: return invalid;
			case TextValidationResult.Success:
				Value = value.Trim();
				return new TextValidationResult.Success();
			default: throw new UnreachableException();
		}
	}

	public override string ToString() => Value;
}