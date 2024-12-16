<script setup lang="ts">
import { apiClient } from '@/scripts/http/Clients';
import { GetExerciseInfoResponse } from '@/scripts/schema/Types';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import { enumFlagsValueToStringArray } from '@/scripts/schema/ZodUtils';
import ButtonDialog from '../misc/ButtonDialog.vue';
import CreateExerciseInfoStep from './step/CreateExerciseInfoStep.vue';
import Entity from '../Entity.vue';
import ExerciseInfoStepsList from './step/ExerciseInfoStepsList.vue';
import { useExerciseInfo } from '@/composables/UseExerciseInfo';
import { useExerciseInfoStepKeys } from '@/composables/UseExerciseInfoStepKeys';
import { UUID } from 'crypto';

const props = defineProps<{
  initialExerciseInfo: GetExerciseInfoResponse;
}>();

const { exerciseInfo, update, destroy } = useExerciseInfo(
  props.initialExerciseInfo.id,
  { initialValue: props.initialExerciseInfo }
);

const { stepKeys } = useExerciseInfoStepKeys(exerciseInfo);

const emit = defineEmits<{ deleted: [UUID] }>();
</script>

<template>
  <Entity
    v-if="exerciseInfo"
    class="mx-auto border border-border rounded-xl flex flex-col gap-6 p-8"
    @deleted="
      destroy();
      emit('deleted', exerciseInfo.id);
      exerciseInfo = undefined;
    "
  >
    <h3>{{ exerciseInfo.name }}</h3>

    <picture v-if="exerciseInfo.thumbnailUrl">
      <source :srcset="`${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`" />
      <img />
    </picture>

    <p>{{ exerciseInfo.description }}</p>

    <div>
      <h4 class="mb-2">Allowed Metric Types</h4>
      <ExerciseMetricTypeToggleGroup
        toggle-type="multiple"
        :model-value="
          enumFlagsValueToStringArray(exerciseInfo.allowedMetricTypes)
        "
        :disabled="true"
      />
    </div>

    <div>
      <h4>Steps</h4>
      <ExerciseInfoStepsList
        :stepKeys="stepKeys"
        @step-deleted="
          key => {
            stepKeys = stepKeys.filter(stepKey => stepKey !== key);
          }
        "
      />
    </div>

    <ButtonDialog dialogTitle="Add Exercise Step">
      <template #button>Add Step</template>
      <template #dialog="{ closeDialog }">
        <CreateExerciseInfoStep
          :exerciseInfoId="exerciseInfo.id"
          @created="
            update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>
  </Entity>
</template>
