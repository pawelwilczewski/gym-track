<script setup lang="ts">
import { apiClient } from '@/shared/http/ApiClient';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import { enumFlagsValueToStringArray } from '@/app/schema/ZodUtils';
import ButtonDialog from '../misc/ButtonDialog.vue';
import CreateExerciseInfoStepForm from './step/CreateExerciseInfoStepForm.vue';
import Entity from '../Entity.vue';
import ExerciseInfoStepsList from './step/ExerciseInfoStepsList.vue';
import { useExerciseInfoStepKeys } from '@/composables/UseExerciseInfoStepKeys';
import { UUID } from 'crypto';
import EditExerciseInfoForm from './EditExerciseInfoForm.vue';
import { useExerciseInfo } from '@/composables/UseExerciseInfo';

const { id } = defineProps<{ id: UUID }>();

const { exerciseInfo, destroy } = useExerciseInfo(id);

const { stepKeys } = useExerciseInfoStepKeys(exerciseInfo);
</script>

<template>
  <Entity v-if="exerciseInfo" class="card" @deleted="destroy()">
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
      <ExerciseInfoStepsList :step-keys="stepKeys" />
    </div>

    <ButtonDialog dialog-title="Add Exercise Step">
      <template #button>Add Step</template>
      <template #dialog="{ closeDialog }">
        <CreateExerciseInfoStepForm
          :exercise-info-id="exerciseInfo.id"
          @created="closeDialog()"
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
        @edited="closeDialog()"
      />
    </template>
  </Entity>
</template>
