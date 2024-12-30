import { apiClient } from '@/app/http/Clients';
import { GetWorkoutResponse } from '@/app/schema/Types';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toastErrorHandler } from '@/app/errors/Handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { useSorted } from '@vueuse/core';

export const useWorkouts = defineStore('workouts', () => {
  const workouts = ref<GetWorkoutResponse[]>([]);

  const sortedWorkouts = useSorted(workouts, (a, b) =>
    a.name.localeCompare(b.name)
  );

  async function fetchWorkouts(): Promise<void> {
    const response = await apiClient.get('/api/v1/workouts');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    workouts.value = response.data;
  }

  return {
    workouts,
    sortedWorkouts,
    fetchWorkouts,
  };
});
