import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toastErrorHandler } from '@/app/errors/Handlers';
import { apiClient } from '@/app/http/Clients';
import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseKey,
} from '@/app/schema/Types';
import { ref, Ref } from 'vue';

export function useWorkoutExercise(
  key: WorkoutExerciseKey,
  options?: { immediate?: boolean }
): {
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined | null>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const workoutExercise = ref<GetWorkoutExerciseResponse | undefined | null>(
    undefined
  );

  if (options?.immediate) {
    update();
  }

  async function update(): Promise<void> {
    const response = await apiClient.get(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    workoutExercise.value = response.data;
  }

  async function destroy(): Promise<void> {
    if (!workoutExercise.value) {
      return;
    }

    const response = await apiClient.delete(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    workoutExercise.value = null;
  }

  return { workoutExercise, update, destroy };
}
