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
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromForm] string description,
		IFormFile? image,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var descriptionOrError = Description.TryFrom(description);
		if (!descriptionOrError.IsSuccess)
		{
			return descriptionOrError.Error.ToValidationProblem(nameof(description));
		}

		var response = await sender.Send(new CreateExerciseInfoStepCommand(
				ExerciseInfoId.From(exerciseInfoId),
				descriptionOrError.ValueObject,
				image?.AsNamedFile(),
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return response.Match<ResultType>(
			success => TypedResults.Created($"{httpContext.Request.Path}/{success.Value.Index}"),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}