import { z } from 'zod';
import { createWorkoutExerciseSetSchema } from './Schemas';
import { ExerciseMetric, ExerciseMetricType } from './Types';

export function createWorkoutExerciseSetSchemaToRequest(
  schema: z.infer<typeof createWorkoutExerciseSetSchema>
): { reps: number; metric: ExerciseMetric } {
  let metric: ExerciseMetric;

  if (schema.metricType === ExerciseMetricType.Distance) {
    metric = {
      $type: ExerciseMetricType.Distance,
      value: schema.distanceValue!,
      units: schema.distanceUnits!,
    };
  } else if (schema.metricType === ExerciseMetricType.Duration) {
    metric = {
      $type: ExerciseMetricType.Duration,
      time: schema.time!,
    };
  } else if (schema.metricType === ExerciseMetricType.Weight) {
    metric = {
      $type: ExerciseMetricType.Weight,
      value: schema.weightValue!,
      units: schema.weightUnits!,
    };
  } else {
    throw new Error(`Invalid request metric type: ${schema.metricType}`);
  }

  return {
    reps: schema.reps,
    metric: metric,
  };
}
