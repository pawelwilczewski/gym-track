import { editExerciseInfoStepSchema } from '@/app/schema/Schemas';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoStepResponse,
  hashExerciseInfoStepKey,
} from '@/app/schema/Types';
import { useExerciseInfoSteps } from '@/app/stores/UseExerciseInfoSteps';
import { FormContext } from 'vee-validate';
import { computed, ComputedRef } from 'vue';
import { z } from 'zod';

export function useExerciseInfoStep(key: ExerciseInfoStepKey): {
  step: ComputedRef<GetExerciseInfoStepResponse | undefined>;
  fetch: () => Promise<boolean>;
  update: (
    data: z.infer<typeof editExerciseInfoStepSchema>,
    form: FormContext
  ) => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const steps = useExerciseInfoSteps();
  const step = computed(() => steps.all[hashExerciseInfoStepKey(key)]);

  async function fetch(): Promise<boolean> {
    return steps.fetchByKey(key);
  }

  async function update(
    data: z.infer<typeof editExerciseInfoStepSchema>,
    form: FormContext
  ): Promise<boolean> {
    return steps.update(key, data, form);
  }

  async function destroy(): Promise<boolean> {
    return steps.destroy(key);
  }

  return { step, fetch, update, destroy };
}
