import { useWorkoutExerciseSets } from '@/features/workout/stores/use-workout-exercise-sets';
import {
  GetWorkoutExerciseSetResponse,
  unhashWorkoutExerciseSetKey,
  WorkoutExerciseKey,
  WorkoutExerciseSetKeyHash,
} from '@/features/workout/types/workout-types';
import { computed, ComputedRef } from 'vue';

export function useSetsOfWorkoutExercise(
  workoutExerciseKey: WorkoutExerciseKey
): ComputedRef<
  Record<WorkoutExerciseSetKeyHash, GetWorkoutExerciseSetResponse>
> {
  const sets = useWorkoutExerciseSets();
  return computed(() => {
    const keys = Object.keys(sets.all).filter(hash => {
      const key = unhashWorkoutExerciseSetKey(hash);
      return (
        key &&
        key.workoutId === workoutExerciseKey.workoutId &&
        key.exerciseIndex === workoutExerciseKey.exerciseIndex
      );
    });

    const result: Record<
      WorkoutExerciseSetKeyHash,
      GetWorkoutExerciseSetResponse
    > = {};

    for (const key of keys) {
      result[key as WorkoutExerciseSetKeyHash] =
        sets.all[key as WorkoutExerciseSetKeyHash];
    }

    return result;
  });
}
