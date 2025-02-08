using Api.Dtos;
using Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<Ok<LoginResponse>, NotFound>;

internal sealed class Login : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/login", async Task<ResultType> (
			[FromBody] LoginRequest login,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new LogInCommand(
					login.Email,
					login.Password), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(new LoginResponse(success.Value.Value)),
				error => TypedResults.NotFound());
		});

		return builder;
	}
}