<script setup lang="ts">
import OneColumnLayout from '@/features/shared/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/features/workout/components/WorkoutsList.vue';
import ButtonDialog from '@/features/shared/components/ButtonDialog.vue';
import CreateWorkoutForm from '@/features/workout/components/CreateWorkoutForm.vue';
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
