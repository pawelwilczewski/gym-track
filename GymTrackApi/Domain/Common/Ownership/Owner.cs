namespace Domain.Common.Ownership;

public abstract record class Owner
{
	public sealed record class User(Guid UserId) : Owner;

	public sealed record class Public : Owner;
}