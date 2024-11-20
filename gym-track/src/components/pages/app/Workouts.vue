<script setup lang="ts">
import CreateWorkout from '@/components/CreateWorkout.vue';
import OneColumnLayout from '@/components/layouts/OneColumnLayout.vue';
import WorkoutsTable from '@/components/WorkoutsTable.vue';
</script>

<template>
  <OneColumnLayout>
    <div>
      <section>
        <h1 class="mb-6">Your Workouts</h1>
        <Suspense>
          <WorkoutsTable />
        </Suspense>
      </section>
    </div>
    <div class="mt-10">
      <CreateWorkout />
    </div>
  </OneColumnLayout>
</template>
