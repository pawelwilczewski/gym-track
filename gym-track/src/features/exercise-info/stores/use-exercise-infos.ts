import { defineStore } from 'pinia';
import { apiClient } from '../../shared/http/api-client';
import { ErrorHandler } from '@/features/shared/errors/error-handler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/handlers';
import { GetExerciseInfoResponse } from '@/features/exercise-info/types/exercise-info-types';
import { ref } from 'vue';
import { UUID } from 'node:crypto';
import { createExerciseInfoSchema } from '@/features/exercise-info/schemas/create-exercise-info-schema';
import { editExerciseInfoSchema } from '@/features/exercise-info/schemas/edit-exercise-info-schema';
import { z } from 'zod';
import { FormContext } from 'vee-validate';
import { toRecord } from '../../shared/utils/conversion-utils';

export const useExerciseInfos = defineStore('exerciseInfos', () => {
  const exerciseInfos = ref<Record<UUID, GetExerciseInfoResponse>>({});

  async function fetchById(id: UUID): Promise<boolean> {
    return fetchByUrl(`/api/v1/exerciseInfos/${id}`);
  }

  async function fetchByUrl(url: string): Promise<boolean> {
    const response = await apiClient.get(url);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    const exerciseInfo: GetExerciseInfoResponse = response.data;
    exerciseInfos.value[exerciseInfo.id] = exerciseInfo;

    return true;
  }

  async function fetchAll(): Promise<boolean> {
    const response = await apiClient.get('/api/v1/exerciseInfos');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    exerciseInfos.value = toRecord(response.data, item => item.id);

    return true;
  }

  async function create(
    data: z.infer<typeof createExerciseInfoSchema>,
    form: FormContext
  ): Promise<boolean> {
    const formData = new FormData();
    formData.append('name', data.name);
    formData.append('description', data.description);
    formData.append('allowedMetricTypes', data.allowedMetricTypes.toString());
    if (data.thumbnailImage) {
      formData.append('thumbnailImage', data.thumbnailImage);
    }

    const response = await apiClient.post('/api/v1/exerciseInfos', formData);

    if (
      ErrorHandler.forResponse(response)
        .handlePartially(formErrorHandler, form)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    return fetchByUrl(response.headers.location);
  }

  async function update(
    id: UUID,
    data: z.infer<typeof editExerciseInfoSchema>,
    form: FormContext
  ): Promise<boolean> {
    const formData = new FormData();
    formData.append('_method', 'PUT');
    formData.append('name', data.name);
    formData.append('description', data.description);
    formData.append('allowedMetricTypes', data.allowedMetricTypes.toString());
    formData.append(
      'replaceThumbnailImage',
      data.replaceThumbnailImage.toString()
    );
    if (data.thumbnailImage) {
      formData.append('thumbnailImage', data.thumbnailImage);
    }

    const response = await apiClient.put(
      `/api/v1/exerciseInfos/${id}`,
      formData
    );

    if (
      ErrorHandler.forResponse(response)
        .handlePartially(formErrorHandler, form)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    return fetchById(id);
  }

  async function destroy(id: UUID): Promise<boolean> {
    const response = await apiClient.delete(`/api/v1/exerciseInfos/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete exerciseInfos.value[id];

    return true;
  }

  return {
    all: exerciseInfos,
    fetchAll,
    fetchById,
    create,
    update,
    destroy,
  };
});
