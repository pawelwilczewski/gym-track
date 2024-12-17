<script setup lang="ts">
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/components/app/workout/WorkoutsList.vue';
import { ref } from 'vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import CreateWorkout from '@/components/app/workout/CreateWorkout.vue';

const workoutsList = ref<typeof WorkoutsList | undefined>(undefined);
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog dialog-title="Create New Workout">
      <template #button>Create Workout</template>
      <template #dialog="{ closeDialog }">
        <CreateWorkout
          @created="
            workoutsList?.update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>
    <section>
      <h1 class="mt-10 mb-6">Your Workouts</h1>
      <WorkoutsList ref="workoutsList" />
    </section>
  </OneColumnLayout>
</template>
