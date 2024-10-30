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
		User = new ClaimsPrincipal()
	};

	// TODO: more appropriate response types (i.e. 201 Created, 403 Forbidden)
	// TODO: ^ return 403 in dev, and in production return 404 using middleware

	// TODO: finish writing tests
}