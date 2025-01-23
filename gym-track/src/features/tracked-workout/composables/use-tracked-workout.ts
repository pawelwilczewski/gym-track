import { UUID } from 'node:crypto';
import { FormContext } from 'vee-validate';
import { computed, ComputedRef } from 'vue';
import { z } from 'zod';
import { GetTrackedWorkoutResponse } from '@/features/tracked-workout/types/tracked-workout-types';
import { editTrackedWorkoutSchema } from '@/features/tracked-workout/schemas/edit-tracked-workout-schema';
import { useTrackedWorkouts } from '@/features/tracked-workout/stores/use-tracked-workouts';

export function useTrackedWorkout(id: UUID): {
  trackedWorkout: ComputedRef<GetTrackedWorkoutResponse | undefined>;
  fetch: () => Promise<boolean>;
  update: (
    data: z.infer<typeof editTrackedWorkoutSchema>,
    form: FormContext
  ) => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const trackedWorkouts = useTrackedWorkouts();
  const trackedWorkout = computed(() => trackedWorkouts.all[id]);

  async function fetch(): Promise<boolean> {
    return trackedWorkouts.fetchById(id);
  }

  async function update(
    data: z.infer<typeof editTrackedWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    return trackedWorkouts.update(id, data, form);
  }

  async function destroy(): Promise<boolean> {
    return trackedWorkouts.destroy(id);
  }

  return { trackedWorkout, fetch, update, destroy };
}
