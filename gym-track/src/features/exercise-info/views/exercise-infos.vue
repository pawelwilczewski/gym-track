<script setup lang="ts">
import CreateExerciseInfoForm from '@/features/exercise-info/components/create-exercise-info-form.vue';
import ExerciseInfosList from '@/features/exercise-info/components/exercise-infos-list.vue';
import LockModeControl from '@/features/lock-mode/components/lock-mode-control.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import FullScreenLayout from '@/features/shared/layouts/full-screen-layout.vue';
</script>

<template>
  <FullScreenLayout>
    <div class="flex justify-between items-center mb-10">
      <h1>Your Exercises</h1>
      <LockModeControl />
    </div>

    <div class="mb-6">
      <ButtonDialog dialog-title="Create New Exercise">
        <template #button>Create Exercise</template>
        <template #dialog="{ closeDialog }">
          <CreateExerciseInfoForm @created="closeDialog()" />
        </template>
      </ButtonDialog>
    </div>

    <ExerciseInfosList />
  </FullScreenLayout>
</template>
