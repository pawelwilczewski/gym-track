import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';

export function useExerciseInfos(): {
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

  return {
    exerciseInfos,
    update,
  };
}
