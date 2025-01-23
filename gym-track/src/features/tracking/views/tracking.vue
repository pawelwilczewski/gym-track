<script setup lang="ts">
import FullScreenLayout from '@/features/shared/layouts/full-screen-layout.vue';
import LockModeControl from '@/features/lock-mode/components/lock-mode-control.vue';
import TrackedWorkoutsList from '@/features/tracked-workout/components/tracked-workouts-list.vue';
</script>

<template>
  <FullScreenLayout>
    <div class="flex justify-between items-center mb-10">
      <h1>Tracking</h1>
      <LockModeControl />
    </div>

    <TrackedWorkoutsList />
  </FullScreenLayout>
</template>
