using Domain.Models.User;

namespace Domain.Common.Ownership;

public abstract record class Owner
{
	public static implicit operator Owner(UserId? userId) =>
		userId == null ? new Public() : new User(userId.Value);

	public static implicit operator UserId?(Owner owner) =>
		owner is User user ? user.UserId : null;

	public sealed record class User(UserId UserId) : Owner;

	public sealed record class Public : Owner;
}