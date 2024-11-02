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

	public static HttpContext User { get; } = new DefaultHttpContext
	{
		User = new ClaimsPrincipal([
			new ClaimsIdentity(ClaimTypes.NameIdentifier)
			{
				// Claims = [new Claim(ClaimTypes.NameIdentifier, "test@test.com")]
			}
		])
	};

	// return 403 in dev, and in production return 404 using middleware
	// TODO: ^^^ document that

	// TODO: finish writing tests
}