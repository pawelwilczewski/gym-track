import {
  GetWorkoutResponse,
  WorkoutExerciseKey,
} from '@/features/workout/types/WorkoutTypes';
import { Ref, ref, watch } from 'vue';

export function useWorkoutExerciseKeys(
  workout: Ref<GetWorkoutResponse | undefined>
): {
  exerciseKeys: Ref<WorkoutExerciseKey[]>;
} {
  const exerciseKeys = ref<WorkoutExerciseKey[]>([]);

  watch(
    workout,
    () => {
      exerciseKeys.value = workout.value?.exercises ?? [];
    },
    { immediate: true }
  );
  return {
    exerciseKeys,
  };
}
