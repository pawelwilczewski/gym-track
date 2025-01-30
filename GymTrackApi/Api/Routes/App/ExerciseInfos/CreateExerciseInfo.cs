using Api.Common;
using Api.Files;
using Application.ExerciseInfo.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos;

using ResultType = Results<Created, ValidationProblem>;

internal sealed class CreateExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<ResultType> (
			HttpContext httpContext,
			[FromForm] string name,
			[FromForm] string description,
			[FromForm] ExerciseMetricType allowedMetricTypes,
			IFormFile? thumbnailImage,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var nameOrError = Name.TryFrom(name);
			if (!nameOrError.IsSuccess)
			{
				return nameOrError.Error.ToValidationProblem(nameof(name));
			}

			var descriptionOrError = Description.TryFrom(description);
			if (!descriptionOrError.IsSuccess)
			{
				return descriptionOrError.Error.ToValidationProblem(nameof(description));
			}

			var metricTypesOrError = SomeExerciseMetricTypes.TryFrom(allowedMetricTypes);
			if (!metricTypesOrError.IsSuccess)
			{
				return metricTypesOrError.Error.ToValidationProblem(nameof(allowedMetricTypes));
			}

			var result = await sender.Send(new CreateExerciseInfoCommand(
					nameOrError.ValueObject,
					descriptionOrError.ValueObject,
					thumbnailImage?.AsNamedFile(),
					metricTypesOrError.ValueObject,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.Created($"{httpContext.Request.Path}/{result.Value.Id}");
		});

		return builder;
	}
}