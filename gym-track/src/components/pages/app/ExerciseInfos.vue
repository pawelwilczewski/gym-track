<script setup lang="ts">
import CreateExerciseInfo from '@/components/app/exerciseInfo/CreateExerciseInfo.vue';
import ExerciseInfosList from '@/components/app/exerciseInfo/ExerciseInfosList.vue';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import { ref } from 'vue';

const exerciseInfosList = ref<typeof ExerciseInfosList | undefined>(undefined);
const createExerciseInfoDialogOpen = ref(false);
const handleExerciseInfoCreated: () => Promise<void> = async () => {
  createExerciseInfoDialogOpen.value = false;
  exerciseInfosList.value?.update();
};
</script>

<template>
  <OneColumnLayout>
    <ButtonDialog
      dialogTitle="Create New Exercise"
      v-model:open="createExerciseInfoDialogOpen"
    >
      <template #button>Create Exercise</template>
      <template #dialog
        ><CreateExerciseInfo @created="handleExerciseInfoCreated"
      /></template>
    </ButtonDialog>
    <section>
      <h1 class="mb-6">Your Exercises</h1>
      <ExerciseInfosList ref="exerciseInfosList" />
    </section>
  </OneColumnLayout>
</template>
