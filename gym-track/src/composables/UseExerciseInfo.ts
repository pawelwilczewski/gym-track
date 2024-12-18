import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { UUID } from 'crypto';

export function useExerciseInfo(
  id: UUID,
  options?: { immediate?: boolean; initialValue?: GetExerciseInfoResponse }
): {
  exerciseInfo: Ref<GetExerciseInfoResponse | undefined | null>;
  update: () => Promise<void>;
  destroy: () => Promise<void>;
} {
  const exerciseInfo = ref<GetExerciseInfoResponse | undefined | null>(
    options?.initialValue
  );

  if (options?.immediate && !options?.initialValue) {
    update();
  }

  async function update(): Promise<void> {
    const response = await apiClient.get(`/api/v1/exerciseInfos/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    exerciseInfo.value = response.data;
  }

  async function destroy(): Promise<void> {
    const response = await apiClient.delete(`/api/v1/exerciseInfos/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    exerciseInfo.value = null;
  }

  return {
    exerciseInfo,
    update,
    destroy,
  };
}
