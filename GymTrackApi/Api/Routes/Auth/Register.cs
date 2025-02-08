using Api.Common;
using Api.Dtos;
using Application.Auth.Commands;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<NoContent, ValidationProblem, Conflict>;

internal sealed class Register : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/register", async Task<ResultType> (
			[FromBody] RegisterRequest request,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var emailOrError = EmailAddress.TryFrom(request.Email);
			if (!emailOrError.IsSuccess) return emailOrError.Error.ToValidationProblem(nameof(request.Email));

			var passwordOrError = Password.TryFrom(request.Password);
			if (!passwordOrError.IsSuccess) return passwordOrError.Error.ToValidationProblem(nameof(request.Password));

			var result = await sender.Send(
					new RegisterCommand(emailOrError.ValueObject, passwordOrError.ValueObject), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				error => TypedResults.Conflict());
		});

		return builder;
	}
}