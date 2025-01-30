using Api.Common;
using Api.Dtos;
using Application.Workout.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

using ResultType = Results<Created, ValidationProblem>;

internal sealed class CreateWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<ResultType> (
			HttpContext httpContext,
			[FromBody] CreateWorkoutRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var nameOrError = Name.TryFrom(request.Name);
			if (!nameOrError.IsSuccess)
			{
				return nameOrError.Error.ToValidationProblem(nameof(request.Name));
			}

			var result = await sender.Send(
					new CreateWorkoutCommand(
						nameOrError.ValueObject,
						httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.Created($"{httpContext.Request.Path}/{result.Value.Id}");
		});

		return builder;
	}
}