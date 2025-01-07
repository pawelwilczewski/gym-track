import { editExerciseInfoStepSchema } from '@/features/exercise-info/schemas/edit-exercise-info-step-schema';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoStepResponse,
  hashExerciseInfoStepKey,
} from '@/features/exercise-info/types/exercise-info-types';
import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';
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
