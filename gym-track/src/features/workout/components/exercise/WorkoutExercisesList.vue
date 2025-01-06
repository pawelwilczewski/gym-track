<script setup lang="ts">
import { WorkoutExerciseKey } from '@/app/schema/Types';
import WorkoutExercise from '@/features/workout/components/exercise/WorkoutExercise.vue';

defineProps<{
  exerciseKeys: WorkoutExerciseKey[];
}>();
</script>

<template>
  <ol class="list-decimal flex flex-col gap-4">
    <WorkoutExercise
      v-for="key in exerciseKeys"
      :key="key.index"
      :exercise-key="key"
    />
  </ol>
</template>
