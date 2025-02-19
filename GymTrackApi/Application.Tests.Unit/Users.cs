using Application.Tests.Unit.Mocks;
using Domain.Common.ValueObjects;
using Domain.Models.User;

namespace Application.Tests.Unit;

internal static class Users
{
	public static UserInfo User0 { get; } = new(UserId.New(), EmailAddress.From("admin1@admin.com"), Password.From("Admin1Password!"));
	public static UserInfo User1 { get; } = new(UserId.New(), EmailAddress.From("user1@user.com"), Password.From("User1Password!"));
	public static UserInfo User2 { get; } = new(UserId.New(), EmailAddress.From("user2@user.com"), Password.From("User2Password!"));
}