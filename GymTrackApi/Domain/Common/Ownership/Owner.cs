namespace Domain.Common.Ownership;

public abstract record class Owner
{
	public static implicit operator Owner(Guid? userId) =>
		userId == null ? new Public() : new User(userId.Value);

	public static implicit operator Guid?(Owner owner) =>
		owner is User user ? user.UserId : null;

	public sealed record class User(Guid UserId) : Owner;

	public sealed record class Public : Owner;
}