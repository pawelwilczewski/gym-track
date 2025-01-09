import {
  GetWorkoutResponse,
  WorkoutExerciseKey,
} from '@/features/workout/types/workout-types';
import { computed, Ref } from 'vue';

export function useWorkoutExerciseKeys(
  workout: Ref<GetWorkoutResponse | undefined>
): Ref<WorkoutExerciseKey[]> {
  return computed(() => workout.value?.exercises ?? []);
}
