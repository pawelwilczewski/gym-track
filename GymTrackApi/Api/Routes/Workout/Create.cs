using Api.Dtos;
using Application.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Workout;

internal sealed class Create : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async (
			[FromBody] CreateWorkout createWorkout,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			dataContext.Workouts.Add(new Domain.Models.Workout.Workout
			{
				Name = createWorkout.Name
			});

			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}