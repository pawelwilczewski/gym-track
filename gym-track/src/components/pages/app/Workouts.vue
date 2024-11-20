<script setup lang="ts">
import CreateWorkout from '@/components/CreateWorkout.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsTable from '@/components/WorkoutsTable.vue';
import { Ref, ref } from 'vue';

const workoutsTable: Ref<typeof WorkoutsTable | undefined> = ref(undefined);
const handleWorkoutCreated: () => Promise<void> = async () => {
  await workoutsTable.value?.update();
};
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Workouts</h1>
        <Suspense>
          <WorkoutsTable ref="workoutsTable" />
        </Suspense>
      </section>
    </div>
    <div class="mt-10">
      <CreateWorkout v-on:created="handleWorkoutCreated" />
    </div>
  </OneColumnLayout>
</template>
