namespace Domain.Models.Common;

public abstract record class CanModifyResult
{
	public sealed record class Ok : CanModifyResult;

	public sealed record class Unauthorized : CanModifyResult;

	public sealed record class NotFound : CanModifyResult;

	public sealed record class ProhibitShared : CanModifyResult;
}