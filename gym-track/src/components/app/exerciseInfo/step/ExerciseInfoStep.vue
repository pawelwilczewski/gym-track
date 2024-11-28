<script setup lang="ts">
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import {
  ExerciseInfoStepKey,
  GetExerciseInfoStepResponse,
} from '@/scripts/schema/Types';
import { ref } from 'vue';

const props = defineProps<{
  stepKey: ExerciseInfoStepKey;
}>();

async function update(): Promise<void> {
  const response = await apiClient.get(
    `/api/v1/exerciseInfos/${props.stepKey.exerciseInfoId}/steps/${props.stepKey.index}`
  );

  if (
    ErrorHandler.forResponse(response).handleFully(toastErrorHandler).wasError()
  ) {
    return;
  }

  step.value = response.data;
}

const step = ref<GetExerciseInfoStepResponse | undefined>(undefined);

update();
</script>

<template>
  <li v-if="step">
    <picture v-if="step.imageUrl">
      <source :srcset="`${apiClient.getUri()}/${step.imageUrl}`" />
      <img />
    </picture>
    <p>{{ step.description }}</p>
  </li>
</template>
