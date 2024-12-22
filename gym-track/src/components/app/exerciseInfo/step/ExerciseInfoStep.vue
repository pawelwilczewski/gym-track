<script setup lang="ts">
import { useExerciseInfoStep } from '@/composables/UseExerciseInfoStep';
import Entity from '../../Entity.vue';
import { apiClient } from '@/app/http/Clients';
import { ExerciseInfoStepKey } from '@/app/schema/Types';
import EditExerciseInfoStepForm from './EditExerciseInfoStepForm.vue';

const props = defineProps<{
  stepKey: ExerciseInfoStepKey;
}>();

const { step, update, destroy } = useExerciseInfoStep(props.stepKey, {
  immediate: true,
});

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
      emit('deleted', props.stepKey);
    "
  >
    <p class="mb-2">{{ step.description }}</p>
    <picture v-if="step.imageUrl" class="mb-2">
      <source :srcset="`${apiClient.getUri()}/${step.imageUrl}`" />
      <img class="step-image mx-auto" />
    </picture>
    <template #edit="{ closeDialog }">
      <EditExerciseInfoStepForm
        :step-key="stepKey"
        :initial-values="{
          description: step.description,
          image:
            step.imageUrl != null
              ? `${apiClient.getUri()}/${step.imageUrl}`
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
