import { editWorkoutSchema } from '@/app/schema/Schemas';
import { GetWorkoutResponse } from '@/app/schema/Types';
import { useWorkouts } from '@/features/workout/stores/UseWorkouts';
import { UUID } from 'crypto';
import { FormContext } from 'vee-validate';
import { computed, ComputedRef } from 'vue';
import { z } from 'zod';

export function useWorkout(id: UUID): {
  workout: ComputedRef<GetWorkoutResponse>;
  fetch: () => Promise<boolean>;
  update: (
    data: z.infer<typeof editWorkoutSchema>,
    form: FormContext
  ) => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const workouts = useWorkouts();
  const workout = computed(() => workouts.all[id]);

  async function fetch(): Promise<boolean> {
    return workouts.fetchById(id);
  }

  async function update(
    data: z.infer<typeof editWorkoutSchema>,
    form: FormContext
  ): Promise<boolean> {
    return workouts.update(id, data, form);
  }

  async function destroy(): Promise<boolean> {
    return workouts.destroy(id);
  }

  return { workout, fetch, update, destroy };
}
