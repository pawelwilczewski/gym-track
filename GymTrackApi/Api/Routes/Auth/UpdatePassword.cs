using Api.Common;
using Api.Dtos;
using Application.Auth.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<NoContent, BadRequest, ValidationProblem>;

internal sealed class UpdatePassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPatch("/update-password", async Task<ResultType> (
				HttpContext httpContext,
				[FromBody] UpdatePasswordRequest request,
				[FromServices] ISender sender,
				CancellationToken cancellationToken) =>
			{
				var oldPasswordOrError = Password.TryFrom(request.OldPassword);
				if (!oldPasswordOrError.IsSuccess) return TypedResults.BadRequest();

				var newPasswordOrError = Password.TryFrom(request.NewPassword);
				if (!newPasswordOrError.IsSuccess) return newPasswordOrError.Error.ToValidationProblem(nameof(request.NewPassword));

				var result = await sender.Send(
						new UpdatePasswordCommand(
							oldPasswordOrError.ValueObject,
							newPasswordOrError.ValueObject,
							httpContext.User.GetUserId()), cancellationToken)
					.ConfigureAwait(false);

				return result.Match<ResultType>(
					success => TypedResults.NoContent(),
					error => TypedResults.BadRequest());
			})
			.RequireAuthorization();

		return builder;
	}
}