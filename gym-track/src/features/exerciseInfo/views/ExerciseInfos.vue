<script setup lang="ts">
import CreateExerciseInfoForm from '@/features/exerciseInfo/components/CreateExerciseInfoForm.vue';
import ExerciseInfosList from '@/features/exerciseInfo/components/ExerciseInfosList.vue';
import ButtonDialog from '@/features/shared/components/ButtonDialog.vue';
import OneColumnLayout from '@/features/shared/layouts/OneColumnLayout.vue';
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
