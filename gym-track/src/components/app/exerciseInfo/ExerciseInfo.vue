<script setup lang="ts">
import { apiClient } from '@/app/http/Clients';
import { GetExerciseInfoResponse } from '@/app/schema/Types';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import { enumFlagsValueToStringArray } from '@/app/schema/ZodUtils';
import ButtonDialog from '../misc/ButtonDialog.vue';
import CreateExerciseInfoStepForm from './step/CreateExerciseInfoStepForm.vue';
import Entity from '../Entity.vue';
import ExerciseInfoStepsList from './step/ExerciseInfoStepsList.vue';
import { useExerciseInfo } from '@/composables/UseExerciseInfo';
import { useExerciseInfoStepKeys } from '@/composables/UseExerciseInfoStepKeys';
import { UUID } from 'crypto';
import { computed } from 'vue';
import EditExerciseInfoForm from './EditExerciseInfoForm.vue';

const props = defineProps<{
  initialExerciseInfo: GetExerciseInfoResponse;
}>();

const { exerciseInfo, update, destroy } = useExerciseInfo(
  computed(() => props.initialExerciseInfo.id),
  { initialValue: props.initialExerciseInfo }
);

const { stepKeys } = useExerciseInfoStepKeys(exerciseInfo);

const emit = defineEmits<{ deleted: [UUID] }>();
</script>

<template>
  <Entity
    v-if="exerciseInfo"
    class="card"
    @deleted="
      emit('deleted', exerciseInfo.id);
      destroy();
    "
  >
    <h3>{{ exerciseInfo.name }}</h3>

    <picture v-if="exerciseInfo.thumbnailUrl">
      <source :srcset="`${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`" />
      <img class="thumbnail-image mx-auto" />
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
        :step-keys="stepKeys"
        @step-deleted="
          key => {
            stepKeys = stepKeys.filter(stepKey => stepKey !== key);
          }
        "
      />
    </div>

    <ButtonDialog dialog-title="Add Exercise Step">
      <template #button>Add Step</template>
      <template #dialog="{ closeDialog }">
        <CreateExerciseInfoStepForm
          :exercise-info-id="exerciseInfo.id"
          @created="
            update();
            closeDialog();
          "
        />
      </template>
    </ButtonDialog>

    <template #edit="{ closeDialog }">
      <EditExerciseInfoForm
        :id="exerciseInfo.id"
        :initial-values="{
          // TODO Pawel: maybe initial-values for edit forms should be handled automatically by edit forms? more single-responsibility?
          description: exerciseInfo.description,
          allowedMetricTypes: enumFlagsValueToStringArray(
            exerciseInfo.allowedMetricTypes
          ),
          name: exerciseInfo.name,
          thumbnailImage:
            exerciseInfo.thumbnailUrl != null
              ? `${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`
              : null,
        }"
        @edited="
          update();
          closeDialog();
        "
      />
    </template>
  </Entity>
</template>
