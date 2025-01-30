using Api.Common;
using Api.Files;
using Application.ExerciseInfo.Step.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos.Steps;

using ResultType = Results<Created, NotFound, ValidationProblem>;

internal sealed class CreateExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromForm] string description,
			IFormFile? image,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var descriptionOrError = Description.TryFrom(description);
			if (!descriptionOrError.IsSuccess)
			{
				return descriptionOrError.Error.ToValidationProblem(nameof(description));
			}

			var result = await sender.Send(new CreateExerciseInfoStepCommand(
					ExerciseInfoId.From(exerciseInfoId),
					descriptionOrError.ValueObject,
					image?.AsNamedFile(),
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Created($"{httpContext.Request.Path}/{success.Value.Index}"),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}