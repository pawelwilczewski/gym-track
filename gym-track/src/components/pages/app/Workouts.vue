<script setup lang="ts">
import CreateWorkout from '@/components/app/workout/CreateWorkout.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/components/app/workout/WorkoutsList.vue';
import { ref } from 'vue';

const workoutsList = ref<typeof WorkoutsList | undefined>(undefined);
function handleWorkoutCreated(): void {
  workoutsList.value?.update();
}
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Workouts</h1>
        <WorkoutsList ref="workoutsList" />
      </section>
    </div>
    <div class="mt-10">
      <CreateWorkout v-on:created="handleWorkoutCreated" />
    </div>
  </OneColumnLayout>
</template>
