using Api.Tests.Mocks;

namespace Api.Tests;

internal static class Users
{
	public static AdminInfo Admin1 { get; } = new(Guid.NewGuid(), "admin1@admin.com", "Admin1Password!");

	public static UserInfo User1 { get; } = new(Guid.NewGuid(), "user1@user.com", "User1Password!");
	public static UserInfo User2 { get; } = new(Guid.NewGuid(), "user2@user.com", "User2Password!");
}