using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Configurations.Converters;

internal sealed class SomeExerciseMetricTypesConverter() : ValueConverter<SomeExerciseMetricTypes, int>(
	metricTypes => (int)metricTypes.Value,
	value => SomeExerciseMetricTypes.From((ExerciseMetricType)value));
