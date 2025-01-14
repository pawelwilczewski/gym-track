<script setup lang="ts">
import WorkoutExercise from '@/features/workout/components/exercise/workout-exercise.vue';
import { UUID } from 'node:crypto';
import { useWorkoutExerciseKeys } from '@/features/workout/composables/use-workout-exercise-keys';
import { useWorkout } from '@/features/workout/composables/use-workout';

const { workoutId } = defineProps<{
  workoutId: UUID;
}>();

const { workout } = useWorkout(workoutId);
const { sortedByDisplayOrder: exerciseKeys } = useWorkoutExerciseKeys(workout);
</script>

<template>
  <ol class="list-decimal flex flex-col">
    <WorkoutExercise
      v-for="key in exerciseKeys"
      :key="key.exerciseIndex"
      :exercise-key="key"
    />
  </ol>
</template>
