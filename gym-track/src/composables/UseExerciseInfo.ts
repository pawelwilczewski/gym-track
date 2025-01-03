import { editExerciseInfoSchema } from '@/app/schema/Schemas';
import { GetExerciseInfoResponse } from '@/app/schema/Types';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';
import { UUID } from 'crypto';
import { FormContext } from 'vee-validate';
import { computed, ComputedRef } from 'vue';
import { z } from 'zod';

export function useExerciseInfo(id: UUID): {
  exerciseInfo: ComputedRef<GetExerciseInfoResponse>;
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
