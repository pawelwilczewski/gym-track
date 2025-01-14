import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';
import {
  ExerciseInfoStepKeyHash,
  GetExerciseInfoStepResponse,
  unhashExerciseInfoStepKey,
} from '@/features/exercise-info/types/exercise-info-types';
import { UUID } from 'node:crypto';
import { computed, ComputedRef } from 'vue';

export function useStepsOfExerciseInfo(
  exerciseInfoId: UUID
): ComputedRef<Record<ExerciseInfoStepKeyHash, GetExerciseInfoStepResponse>> {
  const steps = useExerciseInfoSteps();
  return computed(() => {
    const keys = Object.keys(steps.all).filter(hash => {
      const key = unhashExerciseInfoStepKey(hash as ExerciseInfoStepKeyHash);
      return key && key.exerciseInfoId === exerciseInfoId;
    });

    const result: Record<ExerciseInfoStepKeyHash, GetExerciseInfoStepResponse> =
      {};

    for (const key of keys) {
      result[key as ExerciseInfoStepKeyHash] =
        steps.all[key as ExerciseInfoStepKeyHash];
    }

    return result;
  });
}
