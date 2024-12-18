import {
  ExerciseInfoStepKey,
  GetExerciseInfoResponse,
} from '@/app/schema/Types';
import { Ref, ref, watch } from 'vue';

export function useExerciseInfoStepKeys(
  exerciseInfo: Ref<GetExerciseInfoResponse | undefined | null>
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
