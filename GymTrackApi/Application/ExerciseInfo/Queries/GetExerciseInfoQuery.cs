using Application.ExerciseInfo.Dtos;
using Application.Persistence;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Queries;

using ResultType = OneOf<Success<GetExerciseInfoResponse>, NotFound>;

public sealed record class GetExerciseInfoQuery(
	ExerciseInfoId ExerciseInfoId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetExerciseInfoHandler
	: IRequestHandler<GetExerciseInfoQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetExerciseInfoHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetExerciseInfoQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Readable
			.AsNoTrackingWithIdentityResolution()
			.Include(exerciseInfo => exerciseInfo.Steps)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken);

		if (exerciseInfo is null) return new NotFound();

		return new Success<GetExerciseInfoResponse>(new GetExerciseInfoResponse(
			exerciseInfo.Id.Value,
			exerciseInfo.Name.Value,
			exerciseInfo.Description.Value,
			exerciseInfo.AllowedMetricTypes.Value,
			exerciseInfo.ThumbnailImage?.Value,
			exerciseInfo.Steps
				.Select(step => new ExerciseInfoStepKey(request.ExerciseInfoId.Value, step.Index.Value))
				.ToList()));
	}
}