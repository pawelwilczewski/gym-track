<script setup lang="ts">
import CreateTrackedWorkoutForm from '@/features/tracked-workout/components/create-tracked-workout-form.vue';
import TrackedWorkout from '@/features/tracked-workout/components/tracked-workout.vue';
import { useTrackedWorkouts } from '@/features/tracked-workout/stores/use-tracked-workouts';

const trackedWorkouts = useTrackedWorkouts();
trackedWorkouts.fetchAll();
</script>

<template>
  <CreateTrackedWorkoutForm />
  <TrackedWorkout
    v-for="trackedWorkout in trackedWorkouts.allSorted"
    :key="trackedWorkout.id"
    :tracked-workout-id="trackedWorkout.id"
  />
</template>
