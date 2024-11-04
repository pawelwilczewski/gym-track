namespace Api.Tests.Mocks;

internal interface IUserInfo
{
	Guid Id { get; }
	string Email { get; }
	string Password { get; }
}

internal readonly record struct UserInfo(
	Guid Id,
	string Email,
	string Password) : IUserInfo;

internal readonly record struct AdminInfo(
	Guid Id,
	string Email,
	string Password) : IUserInfo;