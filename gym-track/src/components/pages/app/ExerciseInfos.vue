<script setup lang="ts">
import CreateExerciseInfo from '@/components/app/exerciseInfo/CreateExerciseInfo.vue';
import ExerciseInfosList from '@/components/app/exerciseInfo/ExerciseInfosList.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import { ref } from 'vue';

const exerciseInfosList = ref<typeof ExerciseInfosList | undefined>(undefined);
const handleExerciseInfosCreated: () => Promise<void> = async () => {
  await exerciseInfosList.value?.update();
};
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Exercises</h1>
        <ExerciseInfosList ref="exerciseInfosList" />
      </section>
    </div>
    <div class="mt-10">
      <CreateExerciseInfo v-on:created="handleExerciseInfosCreated" />
    </div>
  </OneColumnLayout>
</template>
