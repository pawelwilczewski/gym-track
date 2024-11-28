<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { toastErrorHandler } from '@/scripts/errors/Handlers';
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import { ref } from 'vue';
import ExerciseInfoStep from './step/ExerciseInfoStep.vue';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import { enumFlagsValueToStringArray } from '@/scripts/schema/ZodUtils';
import ButtonDialog from '../misc/ButtonDialog.vue';
import CreateExerciseInfoStep from './step/CreateExerciseInfoStep.vue';

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

const createStepDialogOpen = ref(false);

function handleStepCreated(): void {
  createStepDialogOpen.value = false;
  update();
}

defineExpose({
  update,
});
</script>

<template>
  <div
    v-if="exerciseInfo"
    class="mx-auto border border-border rounded-xl flex flex-col gap-6 p-8"
  >
    <h3>{{ exerciseInfo.name }}</h3>
    <p>{{ exerciseInfo.description }}</p>
    <div>
      <h4 class="">Allowed Metric Types</h4>
      <ExerciseMetricTypeToggleGroup
        :model-value="
          enumFlagsValueToStringArray(exerciseInfo.allowedMetricTypes)
        "
        :disabled="true"
      />

      <picture>
        <source
          :srcset="`${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`"
        />
        <img />
      </picture>
    </div>
    <div>
      <h4>Steps</h4>
      <ul>
        <ExerciseInfoStep v-for="step in exerciseInfo.steps" :step-key="step" />
      </ul>
    </div>
    <ButtonDialog
      buttonText="Add Step"
      dialogTitle="Add Exercise Step"
      v-model:open="createStepDialogOpen"
    >
      <CreateExerciseInfoStep
        :exerciseInfoId="exerciseInfo.id"
        @created="handleStepCreated"
      />
    </ButtonDialog>

    <Button @click="handleDelete">Delete</Button>
  </div>
</template>
