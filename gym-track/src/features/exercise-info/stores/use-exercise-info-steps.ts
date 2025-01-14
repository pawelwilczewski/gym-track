import { defineStore } from 'pinia';
import { apiClient } from '../../shared/http/api-client';
import { ErrorHandler } from '@/features/shared/errors/error-handler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/handlers';
import {
  ExerciseInfoStepKey,
  ExerciseInfoStepKeyHash,
  GetExerciseInfoStepResponse,
  hashExerciseInfoStepKey,
  unhashExerciseInfoStepKey,
} from '@/features/exercise-info/types/exercise-info-types';
import { ref } from 'vue';
import { UUID } from 'node:crypto';
import { createExerciseInfoStepSchema } from '@/features/exercise-info/schemas/create-exercise-info-step-schema';
import { editExerciseInfoStepSchema } from '@/features/exercise-info/schemas/edit-exercise-info-step-schema';
import { z } from 'zod';
import { FormContext } from 'vee-validate';
import { useExerciseInfos } from './use-exercise-infos';

export const useExerciseInfoSteps = defineStore('exerciseInfoSteps', () => {
  const steps = ref<
    Record<ExerciseInfoStepKeyHash, GetExerciseInfoStepResponse>
  >({});

  const exerciseInfos = useExerciseInfos();

  const keyRegex = new RegExp(/api\/v1\/exerciseInfos\/(.+?)\/steps\/(\d+?)/);

  async function fetchByKey(key: ExerciseInfoStepKey): Promise<boolean> {
    const response = await apiClient.get(
      `/api/v1/exercise-infos/${key.exerciseInfoId}/steps/${key.stepIndex}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    const step: GetExerciseInfoStepResponse = response.data;
    steps.value[hashExerciseInfoStepKey(key)] = step;

    return true;
  }

  async function fetchByUrl(url: string): Promise<boolean> {
    const match = keyRegex.exec(url);

    if (!match) {
      return false;
    }

    const exerciseInfoId = match[1] as UUID;
    const setIndex = Number.parseInt(match[2]);

    return fetchByKey({ exerciseInfoId, stepIndex: setIndex });
  }

  async function fetchMultiple(keys: ExerciseInfoStepKey[]): Promise<boolean> {
    const results = await Promise.all(keys.map(key => fetchByKey(key)));
    return results.every(Boolean);
  }

  async function create(
    exerciseInfoId: UUID,
    data: z.infer<typeof createExerciseInfoStepSchema>,
    form: FormContext
  ): Promise<boolean> {
    const formData = new FormData();
    formData.append('description', data.description);
    if (data.image) {
      formData.append('image', data.image);
    }

    const response = await apiClient.post(
      `/api/v1/exercise-infos/${exerciseInfoId}/steps`,
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

    const result = fetchByUrl(response.headers.location);
    exerciseInfos.fetchById(exerciseInfoId);

    return result;
  }

  async function update(
    key: ExerciseInfoStepKey,
    data: z.infer<typeof editExerciseInfoStepSchema>,
    form: FormContext
  ): Promise<boolean> {
    const formData = new FormData();
    formData.append('_method', 'PUT');
    formData.append('description', data.description);
    formData.append('replaceImage', data.replaceImage.toString());
    if (data.image) {
      formData.append('image', data.image);
    }

    const response = await apiClient.put(
      `/api/v1/exercise-infos/${key.exerciseInfoId}/steps/${key.stepIndex}`,
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

    return fetchByKey(key);
  }

  async function swapDisplayOrders(
    hash1: ExerciseInfoStepKeyHash,
    hash2: ExerciseInfoStepKeyHash
  ): Promise<boolean> {
    const key1 = unhashExerciseInfoStepKey(hash1)!;
    const key2 = unhashExerciseInfoStepKey(hash2)!;

    const promise = Promise.allSettled([
      apiClient.put(
        `/api/v1/exercise-infos/${key1.exerciseInfoId}/steps/${key1.stepIndex}/display-order`,
        { displayOrder: steps.value[hash2].displayOrder }
      ),
      apiClient.put(
        `/api/v1/exercise-infos/${key2.exerciseInfoId}/steps/${key2.stepIndex}/display-order`,
        { displayOrder: steps.value[hash1].displayOrder }
      ),
    ]);

    swap();

    const result = await promise;

    if (result.some(settled => settled.status === 'rejected')) {
      // revert if something went wrong
      swap();
      return false;
    }

    return true;

    function swap(): void {
      [steps.value[hash1], steps.value[hash2]] = [
        {
          ...steps.value[hash1],
          displayOrder: steps.value[hash2].displayOrder,
        },
        {
          ...steps.value[hash2],
          displayOrder: steps.value[hash1].displayOrder,
        },
      ];
    }
  }

  async function destroy(key: ExerciseInfoStepKey): Promise<boolean> {
    const response = await apiClient.delete(
      `/api/v1/exercise-infos/${key.exerciseInfoId}/steps/${key.stepIndex}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete steps.value[hashExerciseInfoStepKey(key)];

    return true;
  }

  return {
    all: steps,
    fetchByKey,
    fetchMultiple,
    create,
    update,
    swapDisplayOrders,
    destroy,
  };
});
