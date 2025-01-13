<script setup lang="ts">
import ReorderArrows from '@/features/shared/components/reorder-arrows.vue';
import { useSortedByDisplayOrderRecord } from '@/features/shared/composables/use-sorted-by-display-order';
import { useExercisesOfWorkout } from '@/features/workout/composables/use-exercises-of-workout';
import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';
import {
  WorkoutExerciseKey,
  WorkoutExerciseKeyHash,
} from '@/features/workout/types/workout-types';

const { exerciseKey } = defineProps<{ exerciseKey: WorkoutExerciseKey }>();

const exercises = useWorkoutExercises();
const workoutExercises = useExercisesOfWorkout(exerciseKey.workoutId);
const sortedExercises = useSortedByDisplayOrderRecord(workoutExercises);

async function reorder(offset: number): Promise<boolean> {
  const entries = Object.entries(sortedExercises.value);
  const indexInSorted = entries.findIndex(
    ([, exercise]) => exercise.index === exerciseKey.exerciseIndex
  );
  const indexWithOffset = indexInSorted + offset;

  if (indexWithOffset < 0 || indexWithOffset >= entries.length) {
    return false;
  }

  return await exercises.swapDisplayOrders(
    entries[indexInSorted][0] as WorkoutExerciseKeyHash,
    entries[indexWithOffset][0] as WorkoutExerciseKeyHash
  );
}
</script>

<template>
  <ReorderArrows @up="reorder(-1)" @down="reorder(1)" />
</template>
