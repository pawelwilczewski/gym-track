<script setup lang="ts">
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/components/app/workout/WorkoutsList.vue';
import { ref } from 'vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import CreateWorkout from '@/components/app/workout/CreateWorkout.vue';

const workoutsList = ref<typeof WorkoutsList | undefined>(undefined);
const createWorkoutDialogOpen = ref(false);
function handleWorkoutCreated(): void {
  createWorkoutDialogOpen.value = false;
  workoutsList.value?.update();
}
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog
      buttonText="Create Workout"
      dialogTitle="Create New Workout"
      v-model:open="createWorkoutDialogOpen"
    >
      <CreateWorkout v-on:created="handleWorkoutCreated" />
    </ButtonDialog>
    <section>
      <h1 class="mt-10 mb-6">Your Workouts</h1>
      <WorkoutsList ref="workoutsList" />
    </section>
  </OneColumnLayout>
</template>
