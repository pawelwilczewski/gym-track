<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';

const workouts: Ref<GetWorkoutResponse[] | undefined> = ref(undefined);

const update: () => Promise<void> = async () => {
  const response = await apiClient.get('/api/v1/workouts');

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  workouts.value = response.data;
};

defineExpose({
  update,
});

await update();
</script>

<template>
  <div v-if="workouts" class="flex flex-col gap-4">
    <Workout v-for="workout in workouts" :workout="workout" />
  </div>
</template>
