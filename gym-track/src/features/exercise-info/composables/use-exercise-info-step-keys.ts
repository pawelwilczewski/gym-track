import {
  ExerciseInfoStepKey,
  GetExerciseInfoResponse,
} from '@/features/exercise-info/types/exercise-info-types';
import { Ref, ref, watch } from 'vue';

export function useExerciseInfoStepKeys(
  exerciseInfo: Ref<GetExerciseInfoResponse | undefined>
): Ref<ExerciseInfoStepKey[]> {
  const stepKeys = ref<ExerciseInfoStepKey[]>([]);

  watch(
    exerciseInfo,
    newExerciseInfo => {
      stepKeys.value = newExerciseInfo?.steps ?? [];
    },
    { immediate: true }
  );

  return stepKeys;
}
