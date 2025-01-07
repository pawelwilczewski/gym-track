import {
  GetWorkoutExerciseResponse,
  hashWorkoutExerciseKey,
  WorkoutExerciseKey,
} from '@/features/workout/types/workout-types';
import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';
import { computed, ComputedRef } from 'vue';

export function useWorkoutExercise(key: WorkoutExerciseKey): {
  workoutExercise: ComputedRef<GetWorkoutExerciseResponse | undefined>;
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
