<script setup lang="ts">
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { Ref, ref } from 'vue';
import ExerciseInfo from './ExerciseInfo.vue';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';

const exerciseInfos: Ref<GetExerciseInfoResponse[] | undefined> =
  ref(undefined);

const update: () => Promise<void> = async () => {
  const response = await apiClient.get('/api/v1/exerciseInfos');
  if (
    !ErrorHandler.forResponse(response).withFull(toastErrorHandler).handle()
  ) {
    return;
  }

  exerciseInfos.value = response.data;
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
