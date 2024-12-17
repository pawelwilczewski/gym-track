import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import {
  GetWorkoutExerciseSetResponse,
  WorkoutExerciseSetKey,
} from '@/scripts/schema/Types';
import { ref, Ref } from 'vue';

export function useWorkoutExerciseSet(
  key: WorkoutExerciseSetKey,
  options?: { immediate?: boolean }
): {
  workoutExerciseSet: Ref<GetWorkoutExerciseSetResponse | undefined | null>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const set = ref<GetWorkoutExerciseSetResponse | undefined | null>(undefined);

  if (options?.immediate) {
    update();
  }

  async function update(): Promise<void> {
    const response = await apiClient.get(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    set.value = response.data;
  }

  async function destroy(): Promise<void> {
    if (!set.value) {
      return;
    }

    const response = await apiClient.delete(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    set.value = null;
  }

  return { workoutExerciseSet: set, update, destroy };
}
