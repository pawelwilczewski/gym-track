<script setup lang="ts">
import { GetWorkoutResponse } from '@/scripts/schema/Schema';

defineProps<{
  workout: GetWorkoutResponse;
}>();
</script>

<template>
  <div
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ workout.name }}</h3>
    <ol>
      <div v-for="exercise in workout.exercises">
        {{ exercise.index }}
        {{ exercise.workoutId }}
      </div>
    </ol>
  </div>
</template>
