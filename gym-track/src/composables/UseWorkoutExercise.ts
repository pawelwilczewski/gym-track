import {
  GetWorkoutExerciseResponse,
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
} from '@/app/schema/Types';
import { useWorkoutExercises } from '@/app/stores/UseWorkoutExercises';
import { computed, ComputedRef } from 'vue';

export function useWorkoutExercise(key: WorkoutExerciseKey): {
  workoutExercise: ComputedRef<GetWorkoutExerciseResponse>;
  fetch: () => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const workoutExercises = useWorkoutExercises();
  const workoutExercise = computed(
    () => workoutExercises.all[hashWorkoutExerciseKey(key)]
  );

  async function fetch(): Promise<boolean> {
    return workoutExercises.fetchByKey(key);
  }

  async function destroy(): Promise<boolean> {
    return workoutExercises.destroy(key);
  }

  return { workoutExercise, fetch, destroy };
}
