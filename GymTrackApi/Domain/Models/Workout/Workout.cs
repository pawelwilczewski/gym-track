namespace Domain.Models.Workout;

public class Workout
{
	public Id<Workout> Id { get; set; }

	public string Name { get; set; }

	public virtual List<Exercise>? Exercises { get; set; }
}