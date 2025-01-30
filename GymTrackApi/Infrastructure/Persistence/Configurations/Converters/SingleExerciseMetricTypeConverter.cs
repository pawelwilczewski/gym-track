using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Configurations.Converters;

internal sealed class SingleExerciseMetricTypeConverter() : ValueConverter<SingleExerciseMetricType, int>(
	metricTypes => (int)metricTypes.Value,
	value => SingleExerciseMetricType.From((ExerciseMetricType)value));