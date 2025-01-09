import {
  GetWorkoutExerciseResponse,
  WorkoutExerciseSetKey,
} from '@/features/workout/types/workout-types';
import { computed, Ref } from 'vue';

export function useWorkoutExerciseSetKeys(
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>
): Ref<WorkoutExerciseSetKey[]> {
  return computed(() => workoutExercise.value?.sets ?? []);
}
