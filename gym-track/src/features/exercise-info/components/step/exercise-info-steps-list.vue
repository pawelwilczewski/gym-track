<script setup lang="ts">
import ExerciseInfoStep from './exercise-info-step.vue';
import { UUID } from 'node:crypto';
import { useExerciseInfoStepKeys } from '@/features/exercise-info/composables/use-exercise-info-step-keys';
import { useExerciseInfo } from '@/features/exercise-info/composables/use-exercise-info';

const { exerciseInfoId } = defineProps<{
  exerciseInfoId: UUID;
}>();

const { exerciseInfo } = useExerciseInfo(exerciseInfoId);
const { sortedByDisplayOrder: stepKeys } =
  useExerciseInfoStepKeys(exerciseInfo);
</script>

<template>
  <ol class="list-decimal">
    <ExerciseInfoStep
      v-for="key in stepKeys"
      :key="key.stepIndex"
      :step-key="key"
    />
  </ol>
</template>
