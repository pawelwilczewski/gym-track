namespace Api.Tests.Mocks;

internal readonly record struct UserInfo(
	Guid Id,
	string Email,
	string Password);