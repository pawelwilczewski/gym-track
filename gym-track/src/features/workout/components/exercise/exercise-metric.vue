<script setup lang="ts">
import {
  ExerciseMetric,
  ExerciseMetricType,
} from '@/features/exercise-info/types/exercise-info-types';
import {
  formatDistance,
  formatWeight,
} from '@/features/shared/utils/formatters';
import { Clock, Route, Weight } from 'lucide-vue-next';

defineProps<{
  exerciseMetric: ExerciseMetric;
}>();
</script>

<template>
  <template v-if="exerciseMetric.$type === ExerciseMetricType.Distance">
    <span class="flex gap-1">
      <Route class="w-4 h-4 my-auto" />
      {{ formatDistance(exerciseMetric.value, exerciseMetric.units) }}
    </span>
  </template>
  <template v-else-if="exerciseMetric.$type === ExerciseMetricType.Duration">
    <span class="flex gap-1">
      <Clock class="w-4 h-4 my-auto" />
      {{ exerciseMetric.time }}
    </span>
  </template>
  <template v-else-if="exerciseMetric.$type === ExerciseMetricType.Weight">
    <span class="flex gap-1">
      <Weight class="w-4 h-4 my-auto" />
      {{ formatWeight(exerciseMetric.value, exerciseMetric.units) }}
    </span>
  </template>
</template>
