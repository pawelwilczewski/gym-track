import { apiClient } from '@/app/http/Clients';
import { GetExerciseInfoResponse } from '@/app/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toastErrorHandler } from '@/app/errors/Handlers';
import { UUID } from 'crypto';

export function useExerciseInfo(
  id: Ref<UUID | undefined>,
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
    if (!id.value) {
      return;
    }

    const response = await apiClient.get(`/api/v1/exerciseInfos/${id.value}`);

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
    if (!id.value) {
      return;
    }

    const response = await apiClient.delete(
      `/api/v1/exerciseInfos/${id.value}`
    );

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
