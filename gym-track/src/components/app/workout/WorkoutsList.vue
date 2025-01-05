<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { useWorkouts } from '@/app/stores/UseWorkouts';

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
