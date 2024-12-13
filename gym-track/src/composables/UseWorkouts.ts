import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';

export function useWorkouts(options?: { immediate?: boolean }): {
  workouts: Ref<GetWorkoutResponse[]>;
  update: () => Promise<void>;
} {
  const workouts = ref<GetWorkoutResponse[]>([]);

  async function update(): Promise<void> {
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

  if (options?.immediate) {
    update();
  }

  return {
    workouts,
    update,
  };
}
