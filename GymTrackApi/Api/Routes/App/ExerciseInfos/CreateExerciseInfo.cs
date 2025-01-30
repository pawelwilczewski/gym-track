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
			var nameResult = Name.TryFrom(name);
			if (!nameResult.IsSuccess)
			{
				return nameResult.Error.ToValidationProblem(nameof(name));
			}

			var descriptionResult = Description.TryFrom(description);
			if (!descriptionResult.IsSuccess)
			{
				return descriptionResult.Error.ToValidationProblem(nameof(description));
			}

			var userId = httpContext.User.GetUserId();

			var result = await sender.Send(new CreateExerciseInfoCommand(
					nameResult.ValueObject,
					descriptionResult.ValueObject,
					thumbnailImage?.AsNamedFile(),
					allowedMetricTypes,
					userId), cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.Created($"{httpContext.Request.Path}/{result.Value.Id}");
		});

		return builder;
	}
}