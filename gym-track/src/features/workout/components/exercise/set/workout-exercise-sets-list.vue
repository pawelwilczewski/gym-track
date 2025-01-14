<script setup lang="ts">
import WorkoutExerciseSet from '@/features/workout/components/exercise/set/workout-exercise-set.vue';
import { useWorkoutExercise } from '@/features/workout/composables/use-workout-exercise';
import { useWorkoutExerciseSetKeys } from '@/features/workout/composables/use-workout-exercise-set-keys';
import { WorkoutExerciseKey } from '@/features/workout/types/workout-types';

const { workoutExerciseKey } = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
}>();

const { workoutExercise } = useWorkoutExercise(workoutExerciseKey);
const { sortedByDisplayOrder: setKeys } =
  useWorkoutExerciseSetKeys(workoutExercise);
</script>

<template>
  <ol class="list-decimal flex flex-col gap-6">
    <WorkoutExerciseSet
      v-for="key in setKeys"
      :key="key.setIndex"
      :set-key="key"
    />
  </ol>
</template>
