<script setup lang="ts">
import CreateExerciseInfo from '@/components/app/exerciseInfo/CreateExerciseInfo.vue';
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
        <CreateExerciseInfo
          @created="
            exerciseInfosList?.update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>
    <section>
      <h1 class="mb-6">Your Exercises</h1>
      <ExerciseInfosList ref="exerciseInfosList" />
    </section>
  </OneColumnLayout>
</template>
