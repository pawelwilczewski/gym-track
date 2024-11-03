using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Mocks;

internal static class HttpContextMocks
{
	public static HttpContext ForAdmin(UserInfo userInfo) => new DefaultHttpContext
	{
		User = new ClaimsPrincipal(
			new ClaimsIdentity([
				new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
				new Claim(ClaimTypes.Name, userInfo.Email),
				new Claim(ClaimTypes.Email, userInfo.Email),
				new Claim(ClaimTypes.Role, Role.ADMINISTRATOR)
			], "mock"))
	};

	public static HttpContext ForUser(UserInfo userInfo) => new DefaultHttpContext
	{
		User = new ClaimsPrincipal(
			new ClaimsIdentity([
				new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
				new Claim(ClaimTypes.Name, userInfo.Email),
				new Claim(ClaimTypes.Email, userInfo.Email)
			], "mock"))
	};
}