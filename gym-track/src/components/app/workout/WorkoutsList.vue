<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { useWorkouts } from '@/app/stores/UseWorkouts';

const store = useWorkouts();
store.fetchWorkouts();
</script>

<template>
  <div class="flex flex-col gap-4">
    <Workout
      v-for="workout in store.sortedWorkouts"
      :key="workout.id"
      :initial-workout="workout"
      @deleted="
        workoutId => {
          // TODO Pawel: this should be abstracted in store
          store.workouts = store.workouts.filter(
            workout => workout.id !== workoutId
          );
        }
      "
    />
  </div>
</template>
