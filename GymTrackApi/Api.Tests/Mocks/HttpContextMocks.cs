using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Mocks;

internal static class HttpContextMocks
{
	public static HttpContext Admin { get; } = new DefaultHttpContext
	{
		User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, Role.ADMINISTRATOR)]))
	};
}