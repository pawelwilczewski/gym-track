using Api.Dtos;
using Application.Auth.Commands;
using Domain.Common.ValueObjects;
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
			HttpContext httpContext,
			[FromBody] LoginRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var emailOrError = EmailAddress.TryFrom(request.Email);
			if (!emailOrError.IsSuccess) return TypedResults.NotFound();

			var passwordOrError = Password.TryFrom(request.Password);
			if (!passwordOrError.IsSuccess) return TypedResults.NotFound();

			var result = await sender.Send(new LogInCommand(
					emailOrError.ValueObject,
					passwordOrError.ValueObject,
					httpContext.Request.Headers.Origin!), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(new LoginResponse(success.Value.Value)),
				error => TypedResults.NotFound());
		});

		return builder;
	}
}