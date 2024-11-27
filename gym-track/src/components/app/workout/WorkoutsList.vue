<script setup lang="ts">
import Workout from '@/components/app/workout/Workout.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { ref } from 'vue';

const workouts = ref<GetWorkoutResponse[]>([]);

async function update(): Promise<void> {
  const response = await apiClient.get('/api/v1/workouts');

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  workouts.value = response.data;
}

defineExpose({
  update,
});

update();
</script>

<template>
  <div class="flex flex-col gap-4">
    <Workout
      v-for="workout in workouts"
      :key="workout.id"
      :initialWorkout="workout"
    />
  </div>
</template>
