import { ErrorHandler } from '@/features/shared/errors/error-handler';
import { toastErrorHandler } from '@/features/shared/errors/handlers';
import { apiClient } from '@/features/shared/http/api-client';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAntiforgery = defineStore('antiforgery', () => {
  const token = ref('');

  async function fetchToken(): Promise<void> {
    const response = await apiClient.get('auth/antiforgery-token');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return;
    }

    token.value = response.data;
  }

  return { token, fetchToken };
});
