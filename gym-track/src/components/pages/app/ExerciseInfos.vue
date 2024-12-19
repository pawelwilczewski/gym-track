<script setup lang="ts">
import CreateExerciseInfoForm from '@/components/app/exerciseInfo/CreateExerciseInfoForm.vue';
import ExerciseInfosList from '@/components/app/exerciseInfo/ExerciseInfosList.vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import { ref } from 'vue';

const exerciseInfosList = ref<typeof ExerciseInfosList | undefined>(undefined);
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog dialog-title="Create New Exercise">
      <template #button>Create Exercise</template>
      <template #dialog="{ closeDialog }">
        <CreateExerciseInfoForm
          @created="
            exerciseInfosList?.update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>
    <section>
      <h1 class="mt-10 mb-6">Your Exercises</h1>
      <ExerciseInfosList ref="exerciseInfosList" />
    </section>
  </OneColumnLayout>
</template>
