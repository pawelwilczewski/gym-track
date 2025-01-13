import { apiClient } from '@/features/shared/http/api-client';
import {
  CreateWorkoutExerciseSetRequest,
  EditWorkoutExerciseSetRequest,
  GetWorkoutExerciseSetResponse,
  hashWorkoutExerciseSetKey,
  unhashWorkoutExerciseSetKey,
  WorkoutExerciseKey,
  WorkoutExerciseSetKey,
  WorkoutExerciseSetKeyHash,
} from '@/features/workout/types/workout-types';
import { ErrorHandler } from '@/features/shared/errors/error-handler';
import {
  formErrorHandler,
  toastErrorHandler,
} from '@/features/shared/errors/handlers';
import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UUID } from 'node:crypto';
import { FormContext } from 'vee-validate';
import { useWorkoutExercises } from './use-workout-exercises';

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
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.setIndex}`
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
      setIndex: setIndex,
    });
  }

  async function fetchMultiple(
    keys: WorkoutExerciseSetKey[]
  ): Promise<boolean> {
    const results = await Promise.all(keys.map(key => fetchByKey(key)));
    return results.every(Boolean);
  }

  async function create(
    workoutExerciseKey: WorkoutExerciseKey,
    data: CreateWorkoutExerciseSetRequest,
    form: FormContext
  ): Promise<boolean> {
    const response = await apiClient.post(
      `/api/v1/workouts/${workoutExerciseKey.workoutId}/exercises/${workoutExerciseKey.exerciseIndex}/sets`,
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
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.setIndex}`,
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

  async function swapDisplayOrders(
    hash1: WorkoutExerciseSetKeyHash,
    hash2: WorkoutExerciseSetKeyHash
  ): Promise<boolean> {
    const key1 = unhashWorkoutExerciseSetKey(hash1)!;
    const key2 = unhashWorkoutExerciseSetKey(hash2)!;

    const promise = Promise.allSettled([
      apiClient.put(
        `/api/v1/workouts/${key1.workoutId}/exercises/${key1.exerciseIndex}/sets/${key1.setIndex}/display-order`,
        { displayOrder: sets.value[hash2].displayOrder }
      ),
      apiClient.put(
        `/api/v1/workouts/${key2.workoutId}/exercises/${key2.exerciseIndex}/sets/${key2.setIndex}/display-order`,
        { displayOrder: sets.value[hash1].displayOrder }
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
      [sets.value[hash1], sets.value[hash2]] = [
        { ...sets.value[hash1], displayOrder: sets.value[hash2].displayOrder },
        { ...sets.value[hash2], displayOrder: sets.value[hash1].displayOrder },
      ];
    }
  }

  async function destroy(key: WorkoutExerciseSetKey): Promise<boolean> {
    const response = await apiClient.delete(
      `/api/v1/workouts/${key.workoutId}/exercises/${key.exerciseIndex}/sets/${key.setIndex}`
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
    swapDisplayOrders,
    destroy,
  };
});
