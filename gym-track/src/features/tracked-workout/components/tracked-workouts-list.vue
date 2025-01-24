<script setup lang="ts">
import TrackedWorkout from '@/features/tracked-workout/components/tracked-workout.vue';
import { useTrackedWorkouts } from '@/features/tracked-workout/stores/use-tracked-workouts';

const trackedWorkouts = useTrackedWorkouts();
trackedWorkouts.fetchAll();
</script>

<template>
  <h2 class="mb-6">Workouts Log</h2>
  <div class="flex flex-col gap-4">
    <TrackedWorkout
      v-for="trackedWorkout in trackedWorkouts.allSorted"
      :key="trackedWorkout.id"
      :tracked-workout-id="trackedWorkout.id"
    />
  </div>
</template>
