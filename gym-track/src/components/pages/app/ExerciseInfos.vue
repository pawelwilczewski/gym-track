<script setup lang="ts">
import CreateExerciseInfoForm from '@/components/app/exerciseInfo/CreateExerciseInfoForm.vue';
import ExerciseInfosList from '@/components/app/exerciseInfo/ExerciseInfosList.vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
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
