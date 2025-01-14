import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';
import {
  GetWorkoutResponse,
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
} from '@/features/workout/types/workout-types';
import { computed, ref, Ref, watch } from 'vue';

export function useWorkoutExerciseKeys(
  workout: Ref<GetWorkoutResponse | undefined>
): {
  exerciseKeys: Ref<WorkoutExerciseKey[]>;
  sortedByDisplayOrder: Ref<WorkoutExerciseKey[]>;
} {
  const exerciseKeys = computed(() => workout.value?.exercises ?? []);

  const exercises = useWorkoutExercises();

  const sortedByDisplayOrder = ref<WorkoutExerciseKey[]>([]);

  watch(exercises.all, async () => {
    if (
      exerciseKeys.value.some(
        key => exercises.all[hashWorkoutExerciseKey(key)] == undefined
      )
    ) {
      return;
    }

    sortedByDisplayOrder.value = [...exerciseKeys.value].sort(
      (a, b) =>
        exercises.all[hashWorkoutExerciseKey(a)].displayOrder -
        exercises.all[hashWorkoutExerciseKey(b)].displayOrder
    );
  });

  watch(exerciseKeys, () => exercises.fetchMultiple(exerciseKeys.value), {
    immediate: true,
  });

  return { exerciseKeys, sortedByDisplayOrder };
}
