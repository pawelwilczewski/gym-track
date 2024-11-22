<script setup lang="ts">
import CreateExerciseInfo from '@/components/app/exerciseInfo/CreateExerciseInfo.vue';
import ExerciseInfosTable from '@/components/app/exerciseInfo/ExerciseInfosTable.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import { ref, Ref } from 'vue';

const exercisesTable: Ref<typeof ExerciseInfosTable | undefined> =
  ref(undefined);
const handleExerciseInfosCreated: () => Promise<void> = async () => {
  await exercisesTable.value?.update();
};
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Exercises</h1>
        <Suspense>
          <ExerciseInfosTable ref="exercisesTable" />
        </Suspense>
      </section>
    </div>
    <div class="mt-10">
      <CreateExerciseInfo v-on:created="handleExerciseInfosCreated" />
    </div>
  </OneColumnLayout>
</template>
