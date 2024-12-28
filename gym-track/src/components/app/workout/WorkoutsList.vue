<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { useSorted } from '@/composables/UseSorted';
import { useWorkouts } from '@/composables/UseWorkouts';

const { workouts, update } = useWorkouts({ immediate: true });

const { sorted: sortedWorkouts } = useSorted(workouts, (a, b) =>
  a.name.localeCompare(b.name)
);

defineExpose({
  update,
});
</script>

<template>
  <div class="flex flex-col gap-4">
    <Workout
      v-for="workout in sortedWorkouts"
      :key="workout.id"
      :initial-workout="workout"
      @deleted="
        workoutId => {
          workouts = workouts.filter(workout => workout.id !== workoutId);
        }
      "
    />
  </div>
</template>
