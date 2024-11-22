<script setup lang="ts">
import { toResult } from '@/scripts/errors/ResponseResult';
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { match, P } from 'ts-pattern';
import { Ref, ref } from 'vue';
import ExerciseInfo from './ExerciseInfo.vue';

const exerciseInfos: Ref<GetExerciseInfoResponse[] | undefined> =
  ref(undefined);

const update: () => Promise<void> = async () => {
  const response = await apiClient.get('/api/v1/exerciseInfos');
  match(toResult(response))
    .with({ type: 'success' }, () => {
      exerciseInfos.value = response.data;
    })
    .with({ type: 'empty' }, () => console.log('Unknown error encountered.'))
    .with({ type: 'message', message: P.select() }, message =>
      console.log(message)
    )
    .with({ type: 'validation', errors: P.select() }, errors => {
      errors.forEach(error => {
        console.log(error);
      });
    })
    .exhaustive();
};

defineExpose({
  update,
});

await update();
</script>

<template>
  <div v-if="exerciseInfos" class="flex flex-col gap-4">
    <ExerciseInfo
      v-for="exerciseInfo in exerciseInfos"
      :exerciseInfo="exerciseInfo"
    />
  </div>
</template>
