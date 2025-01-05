<script setup lang="ts">
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/components/app/workout/WorkoutsList.vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import CreateWorkoutForm from '@/components/app/workout/CreateWorkoutForm.vue';
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog dialog-title="Create New Workout">
      <template #button>Create Workout</template>
      <template #dialog="{ closeDialog }">
        <CreateWorkoutForm @created="closeDialog()" />
      </template>
    </ButtonDialog>
    <section>
      <h1 class="mt-10 mb-6">Your Workouts</h1>
      <WorkoutsList />
    </section>
  </OneColumnLayout>
</template>
