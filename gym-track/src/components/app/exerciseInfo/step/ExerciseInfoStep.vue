<script setup lang="ts">
import { useExerciseInfoStep } from '@/composables/UseExerciseInfoStep';
import Entity from '../../Entity.vue';
import { apiClient } from '@/scripts/http/Clients';
import { ExerciseInfoStepKey } from '@/scripts/schema/Types';

const props = defineProps<{
  stepKey: ExerciseInfoStepKey;
}>();

const { step, destroy } = useExerciseInfoStep(props.stepKey, {
  immediate: true,
});

const emit = defineEmits<{
  deleted: [ExerciseInfoStepKey];
}>();
</script>

<template>
  <Entity
    v-if="step"
    is="li"
    class="my-4"
    @deleted="
      destroy();
      emit('deleted', props.stepKey);
    "
  >
    <p class="mb-2">{{ step.description }}</p>
    <picture v-if="step.imageUrl" class="mb-2">
      <source :srcset="`${apiClient.getUri()}/${step.imageUrl}`" />
      <img />
    </picture>
  </Entity>
</template>
