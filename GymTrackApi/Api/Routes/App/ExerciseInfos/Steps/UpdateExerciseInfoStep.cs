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

using ResultType = Results<NoContent, NotFound, ValidationProblem>;

internal sealed class UpdateExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{stepIndex:int}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int stepIndex,
			[FromForm] string description,
			[FromForm] bool replaceImage,
			IFormFile? image,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var descriptionOrError = Description.TryFrom(description);
			if (!descriptionOrError.IsSuccess)
			{
				return descriptionOrError.Error.ToValidationProblem(nameof(description));
			}

			var result = await sender.Send(new UpdateExerciseInfoStepCommand(
					ExerciseInfoId.From(exerciseInfoId),
					stepIndex,
					descriptionOrError.ValueObject,
					replaceImage,
					image?.AsNamedFile(),
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}