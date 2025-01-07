import { editExerciseInfoSchema } from '@/features/exercise-info/schemas/edit-exercise-info-schema';
import { GetExerciseInfoResponse } from '@/features/exercise-info/types/exercise-info-types';
import { useExerciseInfos } from '@/features/exercise-info/stores/use-exercise-infos';
import { UUID } from 'node:crypto';
import { FormContext } from 'vee-validate';
import { computed, ComputedRef } from 'vue';
import { z } from 'zod';

export function useExerciseInfo(id: UUID): {
  exerciseInfo: ComputedRef<GetExerciseInfoResponse | undefined>;
  fetch: () => Promise<boolean>;
  update: (
    data: z.infer<typeof editExerciseInfoSchema>,
    form: FormContext
  ) => Promise<boolean>;
  destroy: () => Promise<boolean>;
} {
  const exerciseInfos = useExerciseInfos();
  const exerciseInfo = computed(() => exerciseInfos.all[id]);

  async function fetch(): Promise<boolean> {
    return exerciseInfos.fetchById(id);
  }

  async function update(
    data: z.infer<typeof editExerciseInfoSchema>,
    form: FormContext
  ): Promise<boolean> {
    return exerciseInfos.update(id, data, form);
  }

  async function destroy(): Promise<boolean> {
    return exerciseInfos.destroy(id);
  }

  return { exerciseInfo, fetch, update, destroy };
}
