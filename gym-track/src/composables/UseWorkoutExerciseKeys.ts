import { GetWorkoutResponse, WorkoutExerciseKey } from '@/scripts/schema/Types';
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
