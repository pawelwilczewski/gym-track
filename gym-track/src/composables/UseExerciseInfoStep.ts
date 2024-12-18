import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toastErrorHandler } from '@/app/errors/Handlers';
import { apiClient } from '@/app/http/Clients';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoStepResponse,
} from '@/app/schema/Types';
import { ref, Ref } from 'vue';

export function useExerciseInfoStep(
  key: ExerciseInfoStepKey,
  options?: { immediate: boolean }
): {
  step: Ref<GetExerciseInfoStepResponse | undefined | null>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const step = ref<GetExerciseInfoStepResponse | undefined | null>(undefined);

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

    step.value = null;
  }

  return { step, update, destroy };
}
