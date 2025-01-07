import { computed, ComputedRef, Ref, watch } from 'vue';
import { GetWorkoutExerciseResponse } from '@/features/workout/types/workout-types';
import { useExerciseInfos } from '@/features/exercise-info/stores/use-exercise-infos';
import { GetExerciseInfoResponse } from '@/features/exercise-info/types/exercise-info-types';

export function useWorkoutExerciseExerciseInfo(
  workoutExercise: Ref<GetWorkoutExerciseResponse | undefined>
): ComputedRef<GetExerciseInfoResponse | undefined> {
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
      ? exerciseInfos.all[workoutExercise.value.exerciseInfoId]
      : undefined
  );
}
