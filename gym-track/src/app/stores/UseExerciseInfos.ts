import { defineStore } from 'pinia';
import { apiClient } from '../http/Clients';
import { ErrorHandler } from '../errors/ErrorHandler';
import { toastErrorHandler } from '../errors/Handlers';
import { GetExerciseInfoResponse } from '../schema/Types';
import { ref } from 'vue';

export const useExerciseInfos = defineStore('exerciseInfos', () => {
  const exerciseInfos = ref<GetExerciseInfoResponse[]>([]);

  async function fetchExerciseInfos(): Promise<void> {
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
    fetchExerciseInfos,
  };
});
