<script setup lang="ts">
import {
  ExerciseInfoStepKey,
  hashExerciseInfoStepKey,
} from '@/features/exercise-info/types/exercise-info-types';
import ExerciseInfoStep from './exercise-info-step.vue';
import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';
import { asyncComputed } from '@vueuse/core';
import { UUID } from 'node:crypto';
import { useExerciseInfoStepKeys } from '@/features/exercise-info/composables/use-exercise-info-step-keys';
import { useExerciseInfo } from '@/features/exercise-info/composables/use-exercise-info';

const { exerciseInfoId } = defineProps<{
  exerciseInfoId: UUID;
}>();

const { exerciseInfo } = useExerciseInfo(exerciseInfoId);
const stepKeys = useExerciseInfoStepKeys(exerciseInfo);
const steps = useExerciseInfoSteps();

const sortedStepKeys = asyncComputed<ExerciseInfoStepKey[]>(async () => {
  await steps.fetchMultiple(stepKeys.value);
  return [...stepKeys.value].sort(
    (a, b) =>
      steps.all[hashExerciseInfoStepKey(a)].displayOrder -
      steps.all[hashExerciseInfoStepKey(b)].displayOrder
  );
}, []);
</script>

<template>
  <ol class="list-decimal">
    <ExerciseInfoStep
      v-for="key in sortedStepKeys"
      :key="key.stepIndex"
      :step-key="key"
    />
  </ol>
</template>
