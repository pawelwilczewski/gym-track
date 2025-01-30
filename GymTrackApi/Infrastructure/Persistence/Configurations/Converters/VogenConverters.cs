using Domain.Common.ValueObjects;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Tracking;
using Domain.Models.Workout;
using Vogen;

namespace Infrastructure.Persistence.Configurations.Converters;

[EfCoreConverter<WorkoutId>]
[EfCoreConverter<WorkoutExerciseIndex>]
[EfCoreConverter<WorkoutExerciseSetIndex>]
[EfCoreConverter<ExerciseInfoId>]
[EfCoreConverter<ExerciseInfoStepIndex>]
[EfCoreConverter<TrackedWorkoutId>]
[EfCoreConverter<Name>]
[EfCoreConverter<Description>]
[EfCoreConverter<FilePath>]
[EfCoreConverter<Reps>]
internal static partial class VogenConverters;