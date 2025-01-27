namespace Domain.Common.Exceptions;

public sealed class PermissionError() : Exception(
	"Permission Error - this should never be thrown. Fix the code leading to this exception.");