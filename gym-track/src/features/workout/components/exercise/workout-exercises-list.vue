<script setup lang="ts">
import {
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
} from '@/features/workout/types/workout-types';
import WorkoutExercise from '@/features/workout/components/exercise/workout-exercise.vue';
import { UUID } from 'node:crypto';
import { useWorkoutExerciseKeys } from '@/features/workout/composables/use-workout-exercise-keys';
import { useWorkout } from '@/features/workout/composables/use-workout';
import { asyncComputed } from '@vueuse/core';
import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';

const { workoutId } = defineProps<{
  workoutId: UUID;
}>();

const { workout } = useWorkout(workoutId);
const exerciseKeys = useWorkoutExerciseKeys(workout);
const exercises = useWorkoutExercises();

const sortedExerciseKeys = asyncComputed<WorkoutExerciseKey[]>(async () => {
  await exercises.fetchMultiple(exerciseKeys.value);
  return [...exerciseKeys.value].sort(
    (a, b) =>
      exercises.all[hashWorkoutExerciseKey(a)].displayOrder -
      exercises.all[hashWorkoutExerciseKey(b)].displayOrder
  );
}, []);
</script>

<template>
  <ol class="list-decimal flex flex-col gap-4">
    <WorkoutExercise
      v-for="key in sortedExerciseKeys"
      :key="key.index"
      :exercise-key="key"
    />
  </ol>
</template>
