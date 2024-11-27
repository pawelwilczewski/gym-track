<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { ref } from 'vue';

const props = defineProps<{
  initialExerciseInfo: GetExerciseInfoResponse;
}>();

async function update(): Promise<void> {
  if (!exerciseInfo.value) {
    return;
  }

  const response = await apiClient.get(
    `/api/v1/exerciseInfos/${exerciseInfo.value.id}`
  );

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  exerciseInfo.value = response.data;
}

async function handleDelete(): Promise<void> {
  if (!exerciseInfo.value) {
    return;
  }

  const response = await apiClient.delete(
    `/api/v1/exerciseInfos/${exerciseInfo.value.id}`
  );
  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  exerciseInfo.value = undefined;
}

const exerciseInfo = ref<GetExerciseInfoResponse | undefined>(
  props.initialExerciseInfo
);

defineExpose({
  update,
});
</script>

<template>
  <div
    v-if="exerciseInfo"
    class="mx-auto border border-border rounded-xl w-80 flex flex-col gap-6 p-8"
  >
    <h3>{{ exerciseInfo.name }}</h3>
    <p>{{ exerciseInfo.description }}</p>
    <div>{{ exerciseInfo.allowedMetricTypes }}</div>
    <picture>
      <source :srcset="`${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`" />
      <img />
    </picture>
    <ul>
      <li v-for="step in exerciseInfo.steps">
        {{ step.exerciseInfoId }}
        {{ step.index }}
      </li>
    </ul>
    <Button @click="handleDelete">Delete</Button>
  </div>
</template>
