<script setup lang="ts">
import { useExerciseInfoStep } from '@/features/exercise-info/composables/use-exercise-info-step';
import Entity from '@/features/shared/components/entity.vue';
import { apiClient } from '@/features/shared/http/api-client';
import { ExerciseInfoStepKey } from '@/features/exercise-info/types/exercise-info-types';
import EditExerciseInfoStepForm from './edit-exercise-info-step-form.vue';

const { stepKey } = defineProps<{
  stepKey: ExerciseInfoStepKey;
}>();

const { step, destroy } = useExerciseInfoStep(stepKey);
</script>

<template>
  <Entity is="li" v-if="step" class="my-4" @deleted="destroy()">
    <p class="mb-2">{{ step.description }}</p>
    <picture v-if="step.imageUrl" class="mb-2">
      <source :srcset="`${apiClient.getUri()}/${step.imageUrl}`" />
      <img class="step-image mx-auto" />
    </picture>
    <template #edit="{ closeDialog }">
      <EditExerciseInfoStepForm :step-key="stepKey" @edited="closeDialog()" />
    </template>
  </Entity>
</template>
