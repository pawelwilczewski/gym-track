import { GetWorkoutResponse, WorkoutExerciseKey } from '@/app/schema/Types';
import { Ref, ref, watch } from 'vue';

export function useWorkoutExerciseKeys(
  workout: Ref<GetWorkoutResponse | undefined | null>
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
