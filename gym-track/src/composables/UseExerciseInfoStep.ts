import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoStepResponse,
} from '@/scripts/schema/Types';
import { ref, Ref } from 'vue';

export function useExerciseInfoStep(
  key: ExerciseInfoStepKey,
  options?: { immediate: boolean }
): {
  step: Ref<GetExerciseInfoStepResponse | undefined>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const step = ref<GetExerciseInfoStepResponse | undefined>(undefined);

  if (options?.immediate) {
    update();
  }

  async function update(): Promise<void> {
    const response = await apiClient.get(
      `/api/v1/exerciseInfos/${key.exerciseInfoId}/steps/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    step.value = response.data;
  }

  async function destroy(): Promise<void> {
    if (!step.value) {
      return;
    }

    const response = await apiClient.delete(
      `/api/v1/exerciseInfos/${key.exerciseInfoId}/steps/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }
  }

  return { step, update, destroy };
}
