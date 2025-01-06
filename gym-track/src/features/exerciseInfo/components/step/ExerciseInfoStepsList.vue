<script setup lang="ts">
import { ExerciseInfoStepKey } from '@/app/schema/Types';
import ExerciseInfoStep from './ExerciseInfoStep.vue';

defineProps<{
  stepKeys: ExerciseInfoStepKey[];
}>();

const emit = defineEmits<{
  stepDeleted: [ExerciseInfoStepKey];
}>();
</script>

<template>
  <ol class="list-decimal">
    <ExerciseInfoStep
      v-for="key in stepKeys"
      :key="key.index"
      :step-key="key"
      @deleted="emit('stepDeleted', key)"
    />
  </ol>
</template>
