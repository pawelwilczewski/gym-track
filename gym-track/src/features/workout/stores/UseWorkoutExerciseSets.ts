import { apiClient } from '@/features/shared/http/ApiClient';
import {
  CreateWorkoutExerciseSetRequest,
  EditWorkoutExerciseSetRequest,
  GetWorkoutExerciseSetResponse,
  hashWorkoutExerciseSetKey,
  WorkoutExerciseKey,
  WorkoutExerciseSetKey,
  WorkoutExerciseSetKeyHash,
} from '@/features/workout/types/WorkoutTypes';
import { ErrorHandler } from '@/features/shared/errors/ErrorHandler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/Handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UUID } from 'crypto';
import { FormContext } from 'vee-validate';
import { useWorkoutExercises } from './UseWorkoutExercises';

export const useWorkoutExerciseSets = defineStore('workoutExerciseSets', () => {
  const sets = ref<
    Record<WorkoutExerciseSetKeyHash, GetWorkoutExerciseSetResponse>
  >({});

  const workoutExercises = useWorkoutExercises();

  const keyRegex = new RegExp(
    /api\/v1\/workouts\/(.+?)\/exercises\/(\d+?)\/sets\/(\d+?)/
  );

  async function fetchByKey(key: WorkoutExerciseSetKey): Promise<boolean> {
    const response = await apiClient.get(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    const set: GetWorkoutExerciseSetResponse = response.data;
    sets.value[hashWorkoutExerciseSetKey(key)] = set;

    return true;
  }

  async function fetchByUrl(url: string): Promise<boolean> {
    const match = keyRegex.exec(url);

    if (!match) {
      return false;
    }

    const workoutId = match[1] as UUID;
    const exerciseIndex = Number.parseInt(match[2]);
    const setIndex = Number.parseInt(match[3]);

    return fetchByKey({
      workoutId,
      exerciseIndex: exerciseIndex,
      index: setIndex,
    });
  }

  async function fetchMultiple(
    keys: WorkoutExerciseSetKey[]
  ): Promise<boolean> {
    return (await Promise.all(keys.map(key => fetchByKey(key)))).reduce(
      (prev, current) => prev && current,
      true
    );
  }

  async function create(
    workoutExerciseKey: WorkoutExerciseKey,
    data: CreateWorkoutExerciseSetRequest,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post(
      `/api/v1/workouts/${workoutExerciseKey.workoutId}/exercises/${workoutExerciseKey.index}/sets`,
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
    workoutExercises.fetchByKey(workoutExerciseKey);

    return result;
  }

  async function update(
    key: WorkoutExerciseSetKey,
    data: EditWorkoutExerciseSetRequest,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.put(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.index}`,
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

    return fetchByKey(key);
  }

  async function destroy(key: WorkoutExerciseSetKey): Promise<boolean> {
    const response = await apiClient.delete(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.index}`
    );

    if (
      ErrorHandler.forResponse(response)
        .handleFully(toastErrorHandler)
        .wasError()
    ) {
      return false;
    }

    delete sets.value[hashWorkoutExerciseSetKey(key)];

    return true;
  }

  return {
    all: sets,
    fetchByKey,
    fetchMultiple,
    create,
    update,
    destroy,
  };
});
