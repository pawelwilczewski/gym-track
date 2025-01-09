import {
  ExerciseInfoStepKey,
  GetExerciseInfoResponse,
} from '@/features/exercise-info/types/exercise-info-types';
import { computed, Ref } from 'vue';

export function useExerciseInfoStepKeys(
  exerciseInfo: Ref<GetExerciseInfoResponse | undefined>
): Ref<ExerciseInfoStepKey[]> {
  return computed(() => exerciseInfo.value?.steps ?? []);
}
