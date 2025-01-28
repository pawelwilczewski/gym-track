namespace Domain.Common.Ownership;

public interface IOwned
{
	Guid? OwnerId { get; }
	Owner Owner { get; }
}

public static class OwnedExtensions
{
	public static bool CanBeModifiedBy(this IOwned owned, Guid userId) =>
		owned.Owner is Owner.User user && user.UserId == userId;

	public static bool CanBeReadBy(this IOwned owned, Guid userId) =>
		owned.Owner is not Owner.User user || user.UserId == userId;
}