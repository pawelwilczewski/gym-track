<script setup lang="ts">
import { useExerciseInfoStep } from '@/features/exerciseInfo/composables/UseExerciseInfoStep';
import Entity from '@/features/shared/components/Entity.vue';
import { apiClient } from '@/features/shared/http/ApiClient';
import { ExerciseInfoStepKey } from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import EditExerciseInfoStepForm from './EditExerciseInfoStepForm.vue';

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
