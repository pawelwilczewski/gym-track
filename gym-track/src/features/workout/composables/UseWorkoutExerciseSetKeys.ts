import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseSetKey,
} from '@/features/workout/types/WorkoutTypes';
import { Ref, ref, watch } from 'vue';

export function useWorkoutExerciseSetKeys(
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>
): Ref<WorkoutExerciseSetKey[]> {
  const setKeys = ref<WorkoutExerciseSetKey[]>([]);

  watch(
    workoutExercise,
    () => {
      setKeys.value = workoutExercise.value?.sets ?? [];
    },
    { immediate: true }
  );

  return setKeys;
}
