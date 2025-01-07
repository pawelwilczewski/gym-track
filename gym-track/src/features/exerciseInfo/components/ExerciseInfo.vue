<script setup lang="ts">
import { apiClient } from '@/features/shared/http/ApiClient';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';
import { enumFlagsValueToStringArray } from '@/features/shared/utils/ZodUtils';
import ButtonDialog from '@/features/shared/components/ButtonDialog.vue';
import CreateExerciseInfoStepForm from '@/features/exerciseInfo/components/step/CreateExerciseInfoStepForm.vue';
import Entity from '@/features/shared/components/Entity.vue';
import ExerciseInfoStepsList from '@/features/exerciseInfo/components/step/ExerciseInfoStepsList.vue';
import { UUID } from 'crypto';
import EditExerciseInfoForm from './EditExerciseInfoForm.vue';
import { useExerciseInfo } from '@/features/exerciseInfo/composables/UseExerciseInfo';

const { id } = defineProps<{ id: UUID }>();

const { exerciseInfo, destroy } = useExerciseInfo(id);
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
      <ExerciseInfoStepsList :exercise-info-id="id" />
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
      <EditExerciseInfoForm :id="exerciseInfo.id" @edited="closeDialog()" />
    </template>
  </Entity>
</template>
