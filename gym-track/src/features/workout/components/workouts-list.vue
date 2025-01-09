<script setup lang="ts">
import Workout from '@/features/workout/components/workout.vue';
import { useWorkouts } from '@/features/workout/stores/use-workouts';

const workouts = useWorkouts();
workouts.fetchAll();
</script>

<template>
  <div class="flex flex-col gap-4">
    <Workout
      v-for="workout in workouts.allSorted"
      :id="workout.id"
      :key="workout.id"
    />
  </div>
</template>
