import { z } from 'zod';
import {
  ExerciseMetric,
  DistanceUnit,
  ExerciseMetricType,
  WeightUnit,
} from '@/features/exercise-info/types/exercise-info-types';

export const createWorkoutExerciseSetSchema = z
  .object({
    metricType: z.preprocess(Number, z.nativeEnum(ExerciseMetricType)),
    reps: z.number().int().positive(),
    distanceValue: z.number().nonnegative().optional(),
    distanceUnits: z.preprocess(Number, z.nativeEnum(DistanceUnit)).optional(),
    weightValue: z.number().nonnegative().optional(),
    weightUnits: z.preprocess(Number, z.nativeEnum(WeightUnit)).optional(),
    time: z.string().time().optional(),
  })
  .refine(schema => {
    switch (schema.metricType) {
      case ExerciseMetricType.Distance: {
        return (
          schema.distanceValue != undefined && schema.distanceUnits != undefined
        );
      }
      case ExerciseMetricType.Duration: {
        return schema.time;
      }
      case ExerciseMetricType.Weight: {
        return (
          schema.weightValue != undefined && schema.weightUnits != undefined
        );
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

  switch (schema.metricType) {
    case ExerciseMetricType.Distance: {
      metric = {
        $type: ExerciseMetricType.Distance,
        value: schema.distanceValue!,
        units: schema.distanceUnits!,
      };

      break;
    }
    case ExerciseMetricType.Duration: {
      metric = {
        $type: ExerciseMetricType.Duration,
        time: schema.time!,
      };

      break;
    }
    case ExerciseMetricType.Weight: {
      metric = {
        $type: ExerciseMetricType.Weight,
        value: schema.weightValue!,
        units: schema.weightUnits!,
      };

      break;
    }
    case ExerciseMetricType.All: {
      throw new Error('All metric types is not allowed in create set schema.');
    }
    default: {
      throw new Error(`Invalid request metric type: ${schema.metricType}`);
    }
  }

  return {
    reps: schema.reps,
    metric: metric,
  };
}
