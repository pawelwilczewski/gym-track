import { computed, ComputedRef, Ref, watch } from 'vue';
import { GetWorkoutExerciseResponse } from '@/features/workout/types/WorkoutTypes';
import { useExerciseInfos } from '@/features/exerciseInfo/stores/UseExerciseInfos';
import { GetExerciseInfoResponse } from '@/features/exerciseInfo/types/ExerciseInfoTypes';

export function useWorkoutExerciseExerciseInfo(
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>
): ComputedRef<GetExerciseInfoResponse | null> {
  const exerciseInfos = useExerciseInfos();
  watch(
    workoutExercise,
    () => {
      if (
        workoutExercise.value &&
        !exerciseInfos.all[workoutExercise.value.exerciseInfoId]
      ) {
        exerciseInfos.fetchById(workoutExercise.value.exerciseInfoId);
      }
    },
    { immediate: true }
  );

  return computed(() =>
    workoutExercise.value
      ? (exerciseInfos.all[workoutExercise.value.exerciseInfoId] ?? null)
      : null
  );
}
