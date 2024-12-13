<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { useWorkouts } from '@/composables/UseWorkouts';

const { workouts, update } = useWorkouts({ immediate: true });

defineExpose({
  update,
});
</script>

<template>
  <div class="flex flex-col gap-4">
    <Workout
      v-for="workout in workouts"
      :key="workout.id"
      :initialWorkout="workout"
      @deleted="
        workoutId => {
          workouts = workouts.filter(workout => workout.id !== workoutId);
        }
      "
    />
  </div>
</template>
