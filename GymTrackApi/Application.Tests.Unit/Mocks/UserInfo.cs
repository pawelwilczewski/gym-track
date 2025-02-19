using Domain.Common.ValueObjects;
using Domain.Models.User;

namespace Application.Tests.Unit.Mocks;

internal interface IUserInfo
{
	UserId Id { get; }
	EmailAddress Email { get; }
	Password Password { get; }
}

internal readonly record struct UserInfo(
	UserId Id,
	EmailAddress Email,
	Password Password) : IUserInfo;