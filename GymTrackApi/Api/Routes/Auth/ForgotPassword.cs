using Application.Auth.Commands;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = NoContent;

internal sealed class ForgotPassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/forgot-password", async Task<ResultType> (
			[FromBody] ForgotPasswordRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var emailOrError = EmailAddress.TryFrom(request.Email);
			if (!emailOrError.IsSuccess) return TypedResults.NoContent();

			await sender.Send(
					new ForgotPasswordCommand(emailOrError.ValueObject), cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.NoContent();
		});

		return builder;
	}
}