<script setup lang="ts">
import { useExerciseInfoStep } from '@/composables/UseExerciseInfoStep';
import Entity from '../../Entity.vue';
import { apiClient } from '@/shared/http/ApiClient';
import { ExerciseInfoStepKey } from '@/app/schema/Types';
import EditExerciseInfoStepForm from './EditExerciseInfoStepForm.vue';

const { stepKey } = defineProps<{
  stepKey: ExerciseInfoStepKey;
}>();

const { step, fetch, destroy } = useExerciseInfoStep(stepKey);
fetch();

const emit = defineEmits<{
  deleted: [ExerciseInfoStepKey];
}>();
</script>

<template>
  <Entity
    is="li"
    v-if="step"
    class="my-4"
    @deleted="
      destroy();
      emit('deleted', stepKey);
    "
  >
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
