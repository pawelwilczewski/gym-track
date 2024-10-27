using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditWorkoutExerciseSetRequest(
	[property: DataMember] ExerciseMetric Metric,
	[property: DataMember] int Reps);