using Application.ExerciseInfo.Dtos;
using Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;

namespace Application.ExerciseInfo.Queries;

using ResultType = Success<List<GetExerciseInfoResponse>>;

public sealed record class GetExerciseInfosQuery(Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetExerciseInfosHandler
	: IRequestHandler<GetExerciseInfosQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetExerciseInfosHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(GetExerciseInfosQuery request, CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfos = dataContext.ExerciseInfos.Readable
			.Include(exerciseInfo => exerciseInfo.Steps)
			.AsNoTrackingWithIdentityResolution()
			.Select(exerciseInfo => new GetExerciseInfoResponse(
				exerciseInfo.Id.Value,
				exerciseInfo.Name.Value,
				exerciseInfo.Description.Value,
				exerciseInfo.AllowedMetricTypes.Value,
				exerciseInfo.ThumbnailImage != null ? exerciseInfo.ThumbnailImage.Value.Value : null,
				exerciseInfo.Steps
					.Select(step => new ExerciseInfoStepKey(exerciseInfo.Id.Value, step.Index.Value))
					.ToList()));

		return new Success<List<GetExerciseInfoResponse>>(await exerciseInfos
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}
}