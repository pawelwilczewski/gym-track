import { useWorkoutExercises } from '@/features/workout/stores/use-workout-exercises';
import {
  GetWorkoutExerciseResponse,
  unhashWorkoutExerciseKey,
  WorkoutExerciseKeyHash,
} from '@/features/workout/types/workout-types';
import { UUID } from 'node:crypto';
import { computed, ComputedRef } from 'vue';

export function useExercisesOfWorkout(
  workoutId: UUID
): ComputedRef<Record<WorkoutExerciseKeyHash, GetWorkoutExerciseResponse>> {
  const exercises = useWorkoutExercises();
  return computed(() => {
    const keys = Object.keys(exercises.all).filter(hash => {
      const key = unhashWorkoutExerciseKey(hash as WorkoutExerciseKeyHash);
      return key && key.workoutId === workoutId;
    });

    const result: Record<WorkoutExerciseKeyHash, GetWorkoutExerciseResponse> =
      {};

    for (const key of keys) {
      result[key as WorkoutExerciseKeyHash] =
        exercises.all[key as WorkoutExerciseKeyHash];
    }

    return result;
  });
}
