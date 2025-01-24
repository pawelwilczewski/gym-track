<script setup lang="ts">
import FullScreenLayout from '@/features/shared/layouts/full-screen-layout.vue';
import LockModeControl from '@/features/lock-mode/components/lock-mode-control.vue';
import TrackedWorkoutsList from '@/features/tracked-workout/components/tracked-workouts-list.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import CreateTrackedWorkoutForm from '@/features/tracked-workout/components/create-tracked-workout-form.vue';
</script>

<template>
  <FullScreenLayout>
    <div class="flex justify-between items-center mb-10">
      <h1>Tracking</h1>
      <LockModeControl />
    </div>

    <div class="mb-6">
      <ButtonDialog dialog-title="Create Tracked Workout">
        <template #button>Create Tracked Workout</template>
        <template #dialog="{ closeDialog }">
          <CreateTrackedWorkoutForm @created="closeDialog()" />
        </template>
      </ButtonDialog>
    </div>

    <TrackedWorkoutsList />
  </FullScreenLayout>
</template>
