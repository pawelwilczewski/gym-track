import {
  ExerciseInfoStepKey,
  GetExerciseInfoResponse,
} from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { Ref, ref, watch } from 'vue';

export function useExerciseInfoStepKeys(
  exerciseInfo: Ref<GetExerciseInfoResponse | null>
): {
  stepKeys: Ref<ExerciseInfoStepKey[]>;
} {
  const stepKeys = ref<ExerciseInfoStepKey[]>([]);

  watch(
    exerciseInfo,
    newExerciseInfo => {
      stepKeys.value = newExerciseInfo?.steps ?? [];
    },
    { immediate: true }
  );

  return {
    stepKeys,
  };
}
