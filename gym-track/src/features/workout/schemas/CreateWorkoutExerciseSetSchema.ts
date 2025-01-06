import { z } from 'zod';
import {
  ExerciseMetric,
  DistanceUnit,
  ExerciseMetricType,
  WeightUnit,
} from '@/features/exerciseInfo/types/ExerciseInfoTypes';

export const createWorkoutExerciseSetSchema = z
  .object({
    metricType: z.preprocess(
      val => Number(val),
      z.nativeEnum(ExerciseMetricType)
    ),
    reps: z.number().int().positive(),
    distanceValue: z.number().positive().optional(),
    distanceUnits: z
      .preprocess(val => Number(val), z.nativeEnum(DistanceUnit))
      .optional(),
    weightValue: z.number().positive().optional(),
    weightUnits: z
      .preprocess(val => Number(val), z.nativeEnum(WeightUnit))
      .optional(),
    time: z.string().time().optional(),
  })
  .refine(schema => {
    switch (schema.metricType) {
      case ExerciseMetricType.Distance: {
        return schema.distanceValue != null && schema.distanceUnits != null;
      }
      case ExerciseMetricType.Duration: {
        return schema.time;
      }
      case ExerciseMetricType.Weight: {
        return schema.weightValue != null && schema.weightUnits != null;
      }
      case ExerciseMetricType.All: {
        return false;
      }
    }
  }, 'All values are required.');

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
