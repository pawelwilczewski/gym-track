<script setup lang="ts">
import CreateExerciseInfoForm from '@/features/exercise-info/components/create-exercise-info-form.vue';
import ExerciseInfosList from '@/features/exercise-info/components/exercise-infos-list.vue';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import OneColumnLayout from '@/features/shared/layouts/one-column-layout.vue';
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog dialog-title="Create New Exercise">
      <template #button>Create Exercise</template>
      <template #dialog="{ closeDialog }">
        <CreateExerciseInfoForm @created="closeDialog()" />
      </template>
    </ButtonDialog>
    <section>
      <h1 class="mt-10 mb-6">Your Exercises</h1>
      <ExerciseInfosList />
    </section>
  </OneColumnLayout>
</template>
