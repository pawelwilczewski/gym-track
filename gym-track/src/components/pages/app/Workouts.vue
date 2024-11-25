<script setup lang="ts">
import CreateWorkout from '@/components/app/workout/CreateWorkout.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsList from '@/components/app/workout/WorkoutsList.vue';
import { Ref, ref } from 'vue';

const workoutsList: Ref<typeof WorkoutsList | undefined> = ref(undefined);
const handleWorkoutCreated: () => Promise<void> = async () => {
  await workoutsList.value?.update();
};
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Workouts</h1>
        <Suspense>
          <WorkoutsList ref="workoutsList" />
        </Suspense>
      </section>
    </div>
    <div class="mt-10">
      <CreateWorkout v-on:created="handleWorkoutCreated" />
    </div>
  </OneColumnLayout>
</template>
