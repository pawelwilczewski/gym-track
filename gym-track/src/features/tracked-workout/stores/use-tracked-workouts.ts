import { apiClient } from '@/features/shared/http/api-client';
import { ErrorHandler } from '@/features/shared/errors/error-handler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UUID } from 'node:crypto';
import { z } from 'zod';
import { FormContext } from 'vee-validate';
import { toRecord } from '@/features/shared/utils/conversion-utils';
import { useSortedRecord } from '@/features/shared/composables/use-sorted-record';
import { GetTrackedWorkoutResponse } from '@/features/tracked-workout/types/tracked-workout-types';
import { editTrackedWorkoutSchema } from '@/features/tracked-workout/schemas/edit-tracked-workout-schema';
import { createTrackedWorkoutSchema } from '@/features/tracked-workout/schemas/create-tracked-workout-schema';

export const useTrackedWorkouts = defineStore('tracked-workouts', () => {
  const trackedWorkouts = ref<Record<UUID, GetTrackedWorkoutResponse>>({});

  const allSorted = useSortedRecord(trackedWorkouts, (a, b) => {
    return (
      new Date(a.performedAt).getTime() - new Date(b.performedAt).getTime()
    );
  });

  async function fetchAll(): Promise<boolean> {
    const response = await apiClient.get('/api/v1/tracking/workouts');

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    trackedWorkouts.value = toRecord(response.data, item => item.id);

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

    const trackedWorkout = response.data;
    trackedWorkouts.value[trackedWorkout.id] = trackedWorkout;

    return true;
  }

  async function fetchById(id: UUID): Promise<boolean> {
    return fetchByUrl(`/api/v1/tracking/workouts/${id}`);
  }

  async function create(
    data: z.infer<typeof createTrackedWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post('/api/v1/tracking/workouts', data);

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
    data: z.infer<typeof editTrackedWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.put(
      `/api/v1/tracking/workouts/${id}`,
      data
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
    const response = await apiClient.delete(`/api/v1/tracking/workouts/${id}`);

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete trackedWorkouts.value[id];

    return true;
  }

  return {
    all: trackedWorkouts,
    allSorted,
    fetchAll,
    fetchById,
    create,
    update,
    destroy,
  };
});
