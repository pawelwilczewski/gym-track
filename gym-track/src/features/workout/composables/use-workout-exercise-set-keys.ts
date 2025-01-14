import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  GetWorkoutExerciseResponse,
  hashWorkoutExerciseSetKey,
  WorkoutExerciseSetKey,
} from '@/features/workout/types/workout-types';
import { computed, ref, Ref, watch } from 'vue';

export function useWorkoutExerciseSetKeys(
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>
): {
  setKeys: Ref<WorkoutExerciseSetKey[]>;
  sortedByDisplayOrder: Ref<WorkoutExerciseSetKey[]>;
} {
  const setKeys = computed(() => workoutExercise.value?.sets ?? []);

  const sets = useWorkoutExerciseSets();
  const sortedByDisplayOrder = ref<WorkoutExerciseSetKey[]>([]);

  watch(sets.all, async () => {
    if (
      setKeys.value.some(
        key => sets.all[hashWorkoutExerciseSetKey(key)] == undefined
      )
    ) {
      return;
    }

    sortedByDisplayOrder.value = [...setKeys.value].sort(
      (a, b) =>
        sets.all[hashWorkoutExerciseSetKey(a)].displayOrder -
        sets.all[hashWorkoutExerciseSetKey(b)].displayOrder
    );
  });

  watch(setKeys, () => sets.fetchMultiple(setKeys.value), { immediate: true });

  return { setKeys, sortedByDisplayOrder };
}
