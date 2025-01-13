<script setup lang="ts">
import ReorderArrows from '@/features/shared/components/reorder-arrows.vue';
import { useSortedByDisplayOrderRecord } from '@/features/shared/composables/use-sorted-by-display-order';
import { useSetsOfWorkoutExercise } from '@/features/workout/composables/use-sets-of-workout-exercise';
import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  WorkoutExerciseSetKey,
  WorkoutExerciseSetKeyHash,
} from '@/features/workout/types/workout-types';

const { setKey } = defineProps<{ setKey: WorkoutExerciseSetKey }>();

const sets = useWorkoutExerciseSets();
const workoutExerciseSets = useSetsOfWorkoutExercise(setKey);
const sortedSets = useSortedByDisplayOrderRecord(workoutExerciseSets);

async function reorder(offset: number): Promise<boolean> {
  const entries = Object.entries(sortedSets.value);
  const indexInSorted = entries.findIndex(
    ([, set]) => set.index === setKey.setIndex
  );
  const indexWithOffset = indexInSorted + offset;

  if (indexWithOffset < 0 || indexWithOffset >= entries.length) {
    return false;
  }

  return await sets.swapDisplayOrders(
    entries[indexInSorted][0] as WorkoutExerciseSetKeyHash,
    entries[indexWithOffset][0] as WorkoutExerciseSetKeyHash
  );
}
</script>

<template>
  <ReorderArrows @up="reorder(-1)" @down="reorder(1)" />
</template>
