<script setup lang="ts">
import ReorderArrows from '@/features/shared/components/reorder-arrows.vue';
import { useSortedByDisplayOrder } from '@/features/shared/composables/use-sorted-by-display-order';
import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  unhashWorkoutExerciseSetKey,
  WorkoutExerciseSetKey,
  WorkoutExerciseSetKeyHash,
} from '@/features/workout/types/workout-types';
import { computed } from 'vue';

const { setKey } = defineProps<{ setKey: WorkoutExerciseSetKey }>();

const sets = useWorkoutExerciseSets();
const workoutExerciseSets = computed(() =>
  Object.keys(sets.all)
    .filter(hash => {
      const key = unhashWorkoutExerciseSetKey(hash);
      return (
        key &&
        key.workoutId === setKey.workoutId &&
        key.exerciseIndex === setKey.exerciseIndex
      );
    })
    .map(key => sets.all[key as WorkoutExerciseSetKeyHash])
);
const sortedByDisplayOrder = useSortedByDisplayOrder(workoutExerciseSets);
</script>

<template>
  <ReorderArrows
    @up="
      () => {
        const index = sortedByDisplayOrder.findIndex(
          set => set.index === setKey.index
        );

        console.log(index);
        if (index - 1 < 0) {
          return;
        }

        const swap1 = sortedByDisplayOrder[index - 1];
        const swap2 = sortedByDisplayOrder[index];

        const allKeys = Object.keys(
          sets.all
        ) as Array<WorkoutExerciseSetKeyHash>;
        const key1 = allKeys.find(key => sets.all[key] === swap1);

        if (!key1) return;

        const key2 = allKeys.find(key => sets.all[key] === swap2);
        if (!key2) return;

        const temp = sets.all[key1].displayOrder;
        sets.all[key1].displayOrder = sets.all[key2].displayOrder;
        sets.all[key2].displayOrder = temp;

        sets.all[key1] = { ...sets.all[key1] };
        sets.all[key2] = { ...sets.all[key2] };
      }
    "
    @down="
      () => {
        console.log(sortedByDisplayOrder);
        const index = sortedByDisplayOrder.findIndex(
          set => set.index === setKey.index
        );
        console.log(index);
        if (index + 1 >= sortedByDisplayOrder.length) {
          return;
        }
        console.log('swapping down');

        sortedByDisplayOrder[index + 1].displayOrder,
          (sortedByDisplayOrder[index].displayOrder =
            sortedByDisplayOrder[index].displayOrder),
          sortedByDisplayOrder[index + 1].displayOrder;
      }
    "
  />
</template>
