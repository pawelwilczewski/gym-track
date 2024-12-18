import { apiClient } from '@/scripts/http/Clients';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { UUID } from 'crypto';

export function useWorkout(
  id: UUID,
  options?: { immediate?: boolean; initialValue?: GetWorkoutResponse }
): {
  workout: Ref<GetWorkoutResponse | undefined | null>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const workout = ref<GetWorkoutResponse | undefined | null>(
    options?.initialValue
  );

  if (options?.immediate && !options?.initialValue) {
    update();
  }

  async function update(): Promise<void> {
    const response = await apiClient.get(`/api/v1/workouts/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    workout.value = response.data;
  }

  async function destroy(): Promise<void> {
    const response = await apiClient.delete(`/api/v1/workouts/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    workout.value = null;
  }

  return {
    workout,
    update,
    destroy,
  };
}
