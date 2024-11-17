using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class Logout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/logout", async (
				[FromBody] object _,
				[FromServices] SignInManager<User> signInManager) =>
			{
				await signInManager.SignOutAsync();
				return Results.Ok();
			})
			.RequireAuthorization();

		return builder;
	}
}