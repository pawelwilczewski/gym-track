namespace Api.Dtos;

public sealed record class GetWorkoutsResponse(
	List<GetWorkoutResponse> Workouts);