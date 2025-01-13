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
</script>

<template>
  <ReorderArrows
    @up="
      () => {
        const entries = Object.entries(sortedSets);
        const index = entries.findIndex(
          ([, set]) => set.index === setKey.setIndex
        );

        if (index - 1 < 0) {
          return;
        }

        sets.swapDisplayOrders(
          entries[index][0] as WorkoutExerciseSetKeyHash,
          entries[index - 1][0] as WorkoutExerciseSetKeyHash
        );
      }
    "
    @down="
      () => {
        const entries = Object.entries(sortedSets);
        const index = entries.findIndex(
          ([, set]) => set.index === setKey.setIndex
        );

        if (index + 1 >= entries.length) {
          return;
        }

        sets.swapDisplayOrders(
          entries[index][0] as WorkoutExerciseSetKeyHash,
          entries[index + 1][0] as WorkoutExerciseSetKeyHash
        );
      }
    "
  />
</template>
