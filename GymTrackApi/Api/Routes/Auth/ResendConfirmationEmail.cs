using Application.Auth.Commands;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class ResendConfirmationEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/resend-confirmation-email", async Task<NoContent> (
				HttpContext context,
				[FromServices] ISender sender,
				CancellationToken cancellationToken) =>
			{
				await sender
					.Send(new SendConfirmationEmailCommand(context.User.GetUserId()), cancellationToken)
					.ConfigureAwait(false);

				// for no information leak, just indicate Ok
				return TypedResults.NoContent();
			})
			.RequireAuthorization();

		return builder;
	}
}