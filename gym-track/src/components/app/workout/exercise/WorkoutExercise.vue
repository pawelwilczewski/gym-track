<script setup lang="ts">
import { GetWorkoutExerciseResponse } from '@/scripts/schema/Types';

const props = defineProps<{
  exercise: GetWorkoutExerciseResponse;
}>();
</script>

<template>
  <div
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h4>{{ exercise.index }}</h4>
  </div>
</template>
