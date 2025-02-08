using Domain.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class ResetPassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/reset-password", async Task<Results<Ok, ValidationProblem>> (
			[FromBody] ResetPasswordRequest resetRequest,
			[FromServices] UserManager<User> userManager) =>
		{
			throw new NotImplementedException();

			return TypedResults.Ok();
		});

		return builder;
	}
}