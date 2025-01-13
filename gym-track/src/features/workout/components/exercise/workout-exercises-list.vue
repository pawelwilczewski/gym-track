<script setup lang="ts">
import {
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
} from '@/features/workout/types/workout-types';
import WorkoutExercise from '@/features/workout/components/exercise/workout-exercise.vue';
import { UUID } from 'node:crypto';
import { useWorkoutExerciseKeys } from '@/features/workout/composables/use-workout-exercise-keys';
import { useWorkout } from '@/features/workout/composables/use-workout';
import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';
import { ref, watch } from 'vue';

const { workoutId } = defineProps<{
  workoutId: UUID;
}>();

const { workout } = useWorkout(workoutId);
const exerciseKeys = useWorkoutExerciseKeys(workout);
const exercises = useWorkoutExercises();

const sortedExerciseKeys = ref<WorkoutExerciseKey[]>([]);

watch(exercises.all, async () => {
  if (
    exerciseKeys.value.some(
      key => exercises.all[hashWorkoutExerciseKey(key)] == undefined
    )
  ) {
    return;
  }

  sortedExerciseKeys.value = [...exerciseKeys.value].sort(
    (a, b) =>
      exercises.all[hashWorkoutExerciseKey(a)].displayOrder -
      exercises.all[hashWorkoutExerciseKey(b)].displayOrder
  );
});

watch(exerciseKeys, () => exercises.fetchMultiple(exerciseKeys.value), {
  immediate: true,
});
</script>

<template>
  <ol class="list-decimal flex flex-col gap-4">
    <WorkoutExercise
      v-for="key in sortedExerciseKeys"
      :key="key.exerciseIndex"
      :exercise-key="key"
    />
  </ol>
</template>
