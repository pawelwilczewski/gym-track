<script setup lang="ts">
import { useStepsOfExerciseInfo } from '@/features/exercise-info/composables/use-steps-of-exercise-info';
import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';
import {
  ExerciseInfoStepKey,
  ExerciseInfoStepKeyHash,
} from '@/features/exercise-info/types/exercise-info-types';
import ReorderArrows from '@/features/shared/components/reorder-arrows.vue';
import { useSortedByDisplayOrderRecord } from '@/features/shared/composables/use-sorted-by-display-order';

const { stepKey } = defineProps<{ stepKey: ExerciseInfoStepKey }>();

const steps = useExerciseInfoSteps();
const exerciseInfoSteps = useStepsOfExerciseInfo(stepKey.exerciseInfoId);
const sortedSteps = useSortedByDisplayOrderRecord(exerciseInfoSteps);

async function reorder(offset: number): Promise<boolean> {
  const entries = Object.entries(sortedSteps.value);
  const indexInSorted = entries.findIndex(
    ([, step]) => step.index === stepKey.stepIndex
  );
  const indexWithOffset = indexInSorted + offset;

  if (indexWithOffset < 0 || indexWithOffset >= entries.length) {
    return false;
  }

  return await steps.swapDisplayOrders(
    entries[indexInSorted][0] as ExerciseInfoStepKeyHash,
    entries[indexWithOffset][0] as ExerciseInfoStepKeyHash
  );
}
</script>

<template>
  <ReorderArrows @up="reorder(-1)" @down="reorder(1)" />
</template>
