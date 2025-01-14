<script setup lang="ts">
import WorkoutsList from '@/features/workout/components/workouts-list.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import CreateWorkoutForm from '@/features/workout/components/create-workout-form.vue';
import FullScreenLayout from '@/features/shared/layouts/full-screen-layout.vue';
import LockModeControl from '@/features/lock-mode/components/lock-mode-control.vue';
</script>

<template>
  <FullScreenLayout>
    <div class="flex justify-between items-center mb-10">
      <h1>Your Workouts</h1>
      <LockModeControl />
    </div>

    <div class="mb-6">
      <ButtonDialog dialog-title="Create New Workout">
        <template #button>Create Workout</template>
        <template #dialog="{ closeDialog }">
          <CreateWorkoutForm @created="closeDialog()" />
        </template>
      </ButtonDialog>
    </div>

    <WorkoutsList />
  </FullScreenLayout>
</template>
