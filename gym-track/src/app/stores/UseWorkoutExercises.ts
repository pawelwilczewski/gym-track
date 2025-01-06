import { apiClient } from '@/shared/http/ApiClient';
import {
  CreateWorkoutExerciseRequest,
  GetWorkoutExerciseResponse,
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
  WorkoutExerciseKeyHash,
} from '@/app/schema/Types';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UUID } from 'crypto';
import { FormContext } from 'vee-validate';
import { useWorkouts } from './UseWorkouts';

export const useWorkoutExercises = defineStore('workoutExercises', () => {
  const exercises = ref<
    Record<WorkoutExerciseKeyHash, GetWorkoutExerciseResponse>
  >({});

  const workouts = useWorkouts();

  const keyRegex = new RegExp(/api\/v1\/workouts\/(.+?)\/exercises\/(\d+?)/);

  async function fetchByKey(key: WorkoutExerciseKey): Promise<boolean> {
    const response = await apiClient.get(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    const workoutExercise: GetWorkoutExerciseResponse = response.data;
    exercises.value[hashWorkoutExerciseKey(key)] = workoutExercise;

    return true;
  }

  async function fetchByUrl(url: string): Promise<boolean> {
    const match = keyRegex.exec(url);

    if (!match) {
      return false;
    }

    const workoutId: UUID = match[1] as UUID;
    const exerciseIndex: number = Number.parseInt(match[2]);

    return fetchByKey({ workoutId, index: exerciseIndex });
  }

  async function fetchMultiple(keys: WorkoutExerciseKey[]): Promise<boolean> {
    return (await Promise.all(keys.map(key => fetchByKey(key)))).reduce(
      (prev, current) => prev && current,
      true
    );
  }

  async function create(
    workoutId: UUID,
    data: CreateWorkoutExerciseRequest,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post(
      `/api/v1/workouts/${workoutId}/exercises`,
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

    const result = fetchByUrl(response.headers.location);
    workouts.fetchById(workoutId);

    return result;
  }

  async function destroy(key: WorkoutExerciseKey): Promise<boolean> {
    const response = await apiClient.delete(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete exercises.value[hashWorkoutExerciseKey(key)];

    return true;
  }

  return {
    all: exercises,
    fetchByKey,
    fetchMultiple,
    create,
    destroy,
  };
});
