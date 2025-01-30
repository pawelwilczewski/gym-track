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

using ResultType = Results<NoContent, NotFound, ValidationProblem>;

internal sealed class UpdateExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{exerciseInfoId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromForm] string name,
			[FromForm] string description,
			[FromForm] ExerciseMetricType allowedMetricTypes,
			[FromForm] bool replaceThumbnailImage,
			IFormFile? thumbnailImage,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			// TODO Pawel: figure out what to do about this - we don't check in create, would be best if we could have ValueObject based on this enum
			if ((allowedMetricTypes & ExerciseMetricType.All) == 0)
			{
				return TypedResults.ValidationProblem(new Dictionary<string, string[]>
				{
					{ "AllowedMetricTypes", ["At least one metric type must be selected."] }
				});
			}

			var nameOrError = Name.TryFrom(name);
			if (!nameOrError.IsSuccess)
			{
				return nameOrError.Error.ToValidationProblem(nameof(ExerciseInfo.Name));
			}

			var descriptionOrError = Description.TryFrom(description);
			if (!descriptionOrError.IsSuccess)
			{
				return descriptionOrError.Error.ToValidationProblem(nameof(ExerciseInfo.Description));
			}

			var response = await sender.Send(new UpdateExerciseInfoCommand(
					ExerciseInfoId.From(exerciseInfoId),
					nameOrError.ValueObject,
					descriptionOrError.ValueObject,
					replaceThumbnailImage,
					thumbnailImage?.AsNamedFile(),
					allowedMetricTypes,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return response.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}