<script setup lang="ts">
import WorkoutExerciseSet from '@/features/workout/components/exercise/set/workout-exercise-set.vue';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import { useWorkoutExerciseSetKeys } from '@/features/workout/composables/use-workout-exercise-set-keys';
import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  hashWorkoutExerciseSetKey,
  WorkoutExerciseKey,
  WorkoutExerciseSetKey,
} from '@/features/workout/types/workout-types';
import { asyncComputed } from '@vueuse/core';

const { workoutExerciseKey } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
}>();

const { workoutExercise } = useWorkoutExercise(workoutExerciseKey);
const setKeys = useWorkoutExerciseSetKeys(workoutExercise);
const sets = useWorkoutExerciseSets();

const sortedSetKeys = asyncComputed<WorkoutExerciseSetKey[]>(async () => {
  await sets.fetchMultiple(setKeys.value);
  return [...setKeys.value].sort(
    (a, b) =>
      sets.all[hashWorkoutExerciseSetKey(a)].displayOrder -
      sets.all[hashWorkoutExerciseSetKey(b)].displayOrder
  );
}, []);
</script>

<template>
  <ol class="list-decimal flex flex-col gap-6">
    <WorkoutExerciseSet
      v-for="key in sortedSetKeys"
      :key="key.index"
      :set-key="key"
    />
  </ol>
</template>
