using Api.Common;
using Api.Dtos;
using Application.Auth.Commands;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<NoContent, BadRequest, ValidationProblem>;

internal sealed class ResetPassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/reset-password", async Task<ResultType> (
			[FromBody] ResetPasswordRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var codeOrError = PasswordResetCode.TryFrom(request.Code);
			if (!codeOrError.IsSuccess) return TypedResults.BadRequest();

			var passwordOrError = Password.TryFrom(request.NewPassword);
			if (!passwordOrError.IsSuccess) return passwordOrError.Error.ToValidationProblem(nameof(request.NewPassword));

			var result = await sender.Send(
					new ResetPasswordCommand(codeOrError.ValueObject, passwordOrError.ValueObject), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				error => TypedResults.BadRequest());
		});

		return builder;
	}
}