<script setup lang="ts">
import { apiClient } from '@/features/shared/http/api-client';
import ExerciseMetricTypeToggleGroup from './exercise-metric-type-toggle-group.vue';
import { enumFlagsValueToStringArray } from '@/features/shared/utils/zod-utils';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import CreateExerciseInfoStepForm from '@/features/exercise-info/components/step/create-exercise-info-step-form.vue';
import Entity from '@/features/shared/components/entity.vue';
import ExerciseInfoStepsList from '@/features/exercise-info/components/step/exercise-info-steps-list.vue';
import { UUID } from 'node:crypto';
import EditExerciseInfoForm from './edit-exercise-info-form.vue';
import { useExerciseInfo } from '@/features/exercise-info/composables/use-exercise-info';

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
