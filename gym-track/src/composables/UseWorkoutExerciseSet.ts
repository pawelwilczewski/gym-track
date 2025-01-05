import {
  EditWorkoutExerciseSetRequest,
  GetWorkoutExerciseSetResponse,
  hashWorkoutExerciseSetKey,
  WorkoutExerciseSetKey,
} from '@/app/schema/Types';
import { computed, ComputedRef } from 'vue';
import { useWorkoutExerciseSets } from '@/app/stores/UseWorkoutExerciseSets';
import { FormContext } from 'vee-validate';

export function useWorkoutExerciseSet(key: WorkoutExerciseSetKey): {
  set: ComputedRef<GetWorkoutExerciseSetResponse>;
  fetch: () => Promise<boolean>;
  update: (
    data: EditWorkoutExerciseSetRequest,
    form: FormContext
  ) => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const sets = useWorkoutExerciseSets();
  const set = computed(() => sets.all[hashWorkoutExerciseSetKey(key)]);

  async function fetch(): Promise<boolean> {
    return sets.fetchByKey(key);
  }

  async function update(
    data: EditWorkoutExerciseSetRequest,
    form: FormContext
  ): Promise<boolean> {
    return sets.update(key, data, form);
  }

  async function destroy(): Promise<boolean> {
    return sets.destroy(key);
  }

  return { set, fetch, update, destroy };
}
