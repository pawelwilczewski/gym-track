import { apiClient } from '@/app/http/Clients';
import { GetExerciseInfoResponse } from '@/app/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toastErrorHandler } from '@/app/errors/Handlers';

export function useExerciseInfos(options?: { immediate?: boolean }): {
  exerciseInfos: Ref<GetExerciseInfoResponse[]>;
  update: () => Promise<void>;
} {
  const exerciseInfos = ref<GetExerciseInfoResponse[]>([]);

  async function update(): Promise<void> {
    const response = await apiClient.get('/api/v1/exerciseInfos');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    exerciseInfos.value = response.data;
  }

  if (options?.immediate) {
    update();
  }

  return {
    exerciseInfos,
    update,
  };
}
