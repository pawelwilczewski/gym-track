<script setup lang="ts">
import {
  ExerciseInfoStepKey,
  hashExerciseInfoStepKey,
} from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import ExerciseInfoStep from './ExerciseInfoStep.vue';
import { useExerciseInfoSteps } from '@/features/exerciseInfo/stores/UseExerciseInfoSteps';
import { asyncComputed } from '@vueuse/core';
import { UUID } from 'crypto';
import { useExerciseInfoStepKeys } from '@/features/exerciseInfo/composables/UseExerciseInfoStepKeys';
import { useExerciseInfo } from '@/features/exerciseInfo/composables/UseExerciseInfo';

const { exerciseInfoId } = defineProps<{
  exerciseInfoId: UUID;
}>();

const { exerciseInfo } = useExerciseInfo(exerciseInfoId);
const { stepKeys } = useExerciseInfoStepKeys(exerciseInfo);
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
      :key="key.index"
      :step-key="key"
    />
  </ol>
</template>
