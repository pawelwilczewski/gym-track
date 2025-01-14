import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoResponse,
  hashExerciseInfoStepKey,
} from '@/features/exercise-info/types/exercise-info-types';
import { computed, ref, Ref, watch } from 'vue';

export function useExerciseInfoStepKeys(
  exerciseInfo: Ref<GetExerciseInfoResponse | undefined>
): {
  stepKeys: Ref<ExerciseInfoStepKey[]>;
  sortedByDisplayOrder: Ref<ExerciseInfoStepKey[]>;
} {
  const stepKeys = computed(() => exerciseInfo.value?.steps ?? []);

  const steps = useExerciseInfoSteps();
  const sortedByDisplayOrder = ref<ExerciseInfoStepKey[]>([]);

  watch(steps.all, async () => {
    if (
      stepKeys.value.some(
        key => steps.all[hashExerciseInfoStepKey(key)] == undefined
      )
    ) {
      return;
    }

    sortedByDisplayOrder.value = [...stepKeys.value].sort(
      (a, b) =>
        steps.all[hashExerciseInfoStepKey(a)].displayOrder -
        steps.all[hashExerciseInfoStepKey(b)].displayOrder
    );
  });

  watch(stepKeys, () => steps.fetchMultiple(stepKeys.value), {
    immediate: true,
  });

  return { stepKeys, sortedByDisplayOrder };
}
