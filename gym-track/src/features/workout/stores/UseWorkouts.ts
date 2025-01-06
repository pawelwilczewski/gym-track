import { apiClient } from '@/features/shared/http/ApiClient';
import { GetWorkoutResponse } from '@/features/workout/types/WorkoutTypes';
import { ErrorHandler } from '@/features/shared/errors/ErrorHandler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/Handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UUID } from 'crypto';
import { z } from 'zod';
import { createWorkoutSchema } from '@/features/workout/schemas/CreateWorkoutSchema';
import { editWorkoutSchema } from '@/features/workout/schemas/EditWorkoutSchema';
import { FormContext } from 'vee-validate';
import { toRecord } from '@/features/shared/utils/ConversionUtils';
import { useSortedRecord } from '@/features/shared/composables/UseSortedRecord';

export const useWorkouts = defineStore('workouts', () => {
  const workouts = ref<Record<UUID, GetWorkoutResponse>>({});

  const allSorted = useSortedRecord(workouts, (a, b) => {
    return a.name.localeCompare(b.name);
  });

  async function fetchAll(): Promise<boolean> {
    const response = await apiClient.get('/api/v1/workouts');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    workouts.value = toRecord(response.data, item => item.id);

    return true;
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

    const workout = response.data;
    workouts.value[workout.id] = workout;

    return true;
  }

  async function fetchById(id: UUID): Promise<boolean> {
    return fetchByUrl(`/api/v1/workouts/${id}`);
  }

  async function create(
    data: z.infer<typeof createWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post('/api/v1/workouts', data);

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
    data: z.infer<typeof editWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.put(`/api/v1/workouts/${id}`, data);

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
    const response = await apiClient.delete(`/api/v1/workouts/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete workouts.value[id];

    return true;
  }

  return {
    all: workouts,
    allSorted,
    fetchAll,
    fetchById,
    create,
    update,
    destroy,
  };
});
