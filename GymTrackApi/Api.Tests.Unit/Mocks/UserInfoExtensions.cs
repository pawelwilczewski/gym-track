using System.Collections.Concurrent;
using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Mocks;

internal static class UserInfoExtensions
{
	private static readonly ConcurrentDictionary<IUserInfo, HttpContext> cache = new();

	public static HttpContext GetHttpContext(this IUserInfo userInfo) => cache.GetOrAdd(userInfo, CreateHttpContext);

	private static HttpContext CreateHttpContext(IUserInfo userInfo) => new DefaultHttpContext
	{
		User = new ClaimsPrincipal(
			new ClaimsIdentity(
				new List<Claim>
				{
					new(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
					new(ClaimTypes.Name, userInfo.Email),
					new(ClaimTypes.Email, userInfo.Email)
				}.Concat(userInfo is AdminInfo
					? new[] { new Claim(ClaimTypes.Role, Role.ADMINISTRATOR) }
					: Array.Empty<Claim>()),
				"mock"))
	};
}