import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseKey,
} from '@/scripts/schema/Types';
import { ref, Ref } from 'vue';

export function useWorkoutExercise(key: WorkoutExerciseKey): {
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const workoutExercise = ref<GetWorkoutExerciseResponse | undefined>(
    undefined
  );

  update();

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
  }

  return { workoutExercise, update, destroy };
}
