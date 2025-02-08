using System.Diagnostics.CodeAnalysis;
using Domain.Common.Results;

namespace Domain.Common.ValidationExtensions;

internal static class TextValidationExtensions
{
	private static readonly char[] invalidPathCharacters = Path.GetInvalidPathChars();

	public static bool IsNotNull(
		this string? text,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (text is null)
		{
			error = new ValidationError("Text value required.");
			return false;
		}

		error = null;
		return true;
	}

	public static bool IsNotNullOrWhitespace(
		this string? text,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			error = new ValidationError("Text is null or white space.");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMinLength(
		this string text,
		int minLength,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (minLength < 0)
		{
			error = new ValidationError("Text min length must be greater than or equal to 0 (internal error).");
			return false;
		}

		if (text.Length < minLength)
		{
			error = new ValidationError($"Text too short (min {minLength} characters).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMaxLength(
		this string text,
		int maxLength,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (maxLength < 1)
		{
			error = new ValidationError("Text max length must be greater than 0 (internal error).");
			return false;
		}

		if (text.Length > maxLength)
		{
			error = new ValidationError($"Text too long (max {maxLength} characters).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool IsNotOnlyPunctuation(
		this string text,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (text.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c)))
		{
			error = new ValidationError("Text consists of punctuation only.");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasNoInvalidPathCharacters(
		this string text,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (text.IndexOfAny(invalidPathCharacters) >= 0)
		{
			error = new ValidationError("Path contains invalid path characters.");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMinUpperCaseCharacters(
		this string text,
		int minUpperCaseCount,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (minUpperCaseCount >= 0 && text.Count(char.IsUpper) < minUpperCaseCount)
		{
			error = new ValidationError($"Not enough upper case characters (min {minUpperCaseCount}).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMinLowerCaseCharacters(
		this string text,
		int minLowerCaseCount,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (minLowerCaseCount >= 0 && text.Count(char.IsLower) < minLowerCaseCount)
		{
			error = new ValidationError($"Not enough lower case characters (min {minLowerCaseCount}).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMinDigitCharacters(
		this string text,
		int minDigitCount,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (minDigitCount >= 0 && text.Count(char.IsDigit) < minDigitCount)
		{
			error = new ValidationError($"Not enough digit characters (min {minDigitCount}).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasMinSpecialCharacters(
		this string text,
		int minSpecialCount,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (minSpecialCount >= 0 && text.Count(char.IsSymbol) < minSpecialCount)
		{
			error = new ValidationError($"Not enough special characters (min {minSpecialCount}).");
			return false;
		}

		error = null;
		return true;
	}

	public static bool HasNoWhiteSpaceCharacters(
		this string text,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (text.Any(char.IsWhiteSpace))
		{
			error = new ValidationError("Should not have any whitespace characters.");
			return false;
		}

		error = null;
		return true;
	}
}