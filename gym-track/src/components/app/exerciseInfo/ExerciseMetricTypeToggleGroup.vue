<script setup lang="ts">
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { ExerciseMetricType } from '@/scripts/schema/Types';
import { Weight, Clock, Route } from 'lucide-vue-next';
import { SingleOrMultipleType } from 'node_modules/radix-vue/dist/shared/types';
import { PropType } from 'vue';

const { disabled = false, toggleType } = defineProps<{
  disabled?: boolean;
  toggleType: SingleOrMultipleType;
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
    >
      <Weight class="h-4 w-4 mr-1" />
      Weight
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Duration.toString()"
      aria-label="Toggle duration"
    >
      <Clock class="h-4 w-4 mr-1" />
      Duration
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Distance.toString()"
      aria-label="Toggle distance"
    >
      <Route class="h-4 w-4 mr-1" />
      Distance
    </ToggleGroupItem>
  </ToggleGroup>
</template>
