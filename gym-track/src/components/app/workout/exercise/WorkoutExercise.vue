<script setup lang="ts">
import Entity from '../../Entity.vue';
import { GetWorkoutExerciseResponse } from '@/scripts/schema/Types';

defineProps<{
  exercise: GetWorkoutExerciseResponse;
}>();
</script>

<template>
  <Entity class="flex flex-col gap-6 p-8">
    <h4>{{ exercise.index }}</h4>
  </Entity>
</template>
