using Application.Persistence;
using Application.Workout.Dtos;
using Domain.Common.Results;
using Domain.Models.User;
using Domain.Models.Workout;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Workout.Queries;

using ResultType = Result<Success<GetWorkoutResponse>, NotFound>;

public sealed record class GetWorkoutQuery(
	WorkoutId WorkoutId,
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetWorkoutHandler
	: IRequestHandler<GetWorkoutQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetWorkoutQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken);

		if (workout is null) return new NotFound();

		return new Success<GetWorkoutResponse>(new GetWorkoutResponse(
			workout.Id.Value,
			workout.Name.ToString(),
			workout.Exercises.Select(exercise => new WorkoutExerciseKey(workout.Id.Value, exercise.Index.Value))
				.ToList()));
	}
}