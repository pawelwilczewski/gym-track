using Application.Auth.Commands;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class SendConfirmationEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/send-confirmation-email", async Task<NoContent> (
				[FromBody] object _,
				HttpContext context,
				[FromServices] ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender
					.Send(new SendConfirmationEmailCommand(context.User.GetUserId()), cancellationToken)
					.ConfigureAwait(false);

				// for no information leak, just indicate Ok
				return TypedResults.NoContent();
			})
			.RequireAuthorization();

		return builder;
	}
}