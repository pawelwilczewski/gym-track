<script setup lang="ts">
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { ExerciseMetricType } from '@/scripts/schema/Types';
import { Weight, Clock, Route } from 'lucide-vue-next';
import { SingleOrMultipleType } from 'node_modules/radix-vue/dist/shared/types';
import { PropType } from 'vue';

const {
  disabled = false,
  toggleType,
  enabledOptions = Number.MAX_SAFE_INTEGER,
} = defineProps<{
  disabled?: boolean;
  toggleType: SingleOrMultipleType;
  enabledOptions?: number;
}>();

const model = defineModel({
  type: [Array<string>, String] as PropType<Array<string> | string>,
});
</script>

<template>
  <ToggleGroup v-model="model" :type="toggleType" :disabled="disabled">
    <ToggleGroupItem
      :value="ExerciseMetricType.Weight.toString()"
      aria-label="Toggle reps"
      :disabled="(enabledOptions & ExerciseMetricType.Weight) === 0"
    >
      <Weight class="h-4 w-4 mr-1" />
      Weight
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Duration.toString()"
      aria-label="Toggle duration"
      :disabled="(enabledOptions & ExerciseMetricType.Duration) === 0"
    >
      <Clock class="h-4 w-4 mr-1" />
      Duration
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Distance.toString()"
      aria-label="Toggle distance"
      :disabled="(enabledOptions & ExerciseMetricType.Distance) === 0"
    >
      <Route class="h-4 w-4 mr-1" />
      Distance
    </ToggleGroupItem>
  </ToggleGroup>
</template>
