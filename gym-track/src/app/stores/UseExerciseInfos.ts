import { defineStore } from 'pinia';
import { apiClient } from '../http/Clients';
import { ErrorHandler } from '../errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '../errors/Handlers';
import { GetExerciseInfoResponse } from '../schema/Types';
import { ref } from 'vue';
import { UUID } from 'crypto';
import {
  createExerciseInfoSchema,
  editExerciseInfoSchema,
} from '../schema/Schemas';
import { z } from 'zod';
import { FormContext } from 'vee-validate';
import { toRecord } from '../utils/ConversionUtils';

export const useExerciseInfos = defineStore('exerciseInfos', () => {
  const exerciseInfos = ref<Record<UUID, GetExerciseInfoResponse>>({});

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

  async function fetchById(id: UUID): Promise<boolean> {
    const response = await apiClient.get(`/api/v1/exerciseInfos/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    exerciseInfos.value[id] = response.data;

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

    const exerciseInfo: GetExerciseInfoResponse = await apiClient.get(
      response.headers.location
    );
    exerciseInfos.value[exerciseInfo.id] = exerciseInfo;

    return true;
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

    fetchById(id);

    return true;
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
