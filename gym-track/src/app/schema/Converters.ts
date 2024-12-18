import { z } from 'zod';
import { createWorkoutExerciseSetSchema } from './Schemas';
import { ExerciseMetric, ExerciseMetricType } from './Types';

export function createWorkoutExerciseSchemaToRequest(
  schema: z.infer<typeof createWorkoutExerciseSetSchema>
): { reps: number; metric: ExerciseMetric | undefined } {
  return {
    reps: schema.reps,
    metric:
      schema.metricType === ExerciseMetricType.Distance
        ? {
            $type: ExerciseMetricType.Distance,
            value: schema.distanceValue!,
            units: schema.distanceUnits!,
          }
        : schema.metricType === ExerciseMetricType.Duration
          ? {
              $type: ExerciseMetricType.Duration,
              time: schema.time!,
            }
          : schema.metricType === ExerciseMetricType.Weight
            ? {
                $type: ExerciseMetricType.Weight,
                value: schema.weightValue!,
                units: schema.weightUnits!,
              }
            : undefined,
  };
}
