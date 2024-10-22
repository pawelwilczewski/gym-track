namespace Domain.Models.Common;

public abstract record class CantModifyReason
{
	public sealed record class Unauthorized : CantModifyReason;

	public sealed record class NotFound : CantModifyReason;

	public sealed record class ProhibitShared : CantModifyReason;
}