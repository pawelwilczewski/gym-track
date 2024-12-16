<script setup lang="ts">
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { ExerciseMetricType } from '@/scripts/schema/Types';
import { Weight, Clock, Route } from 'lucide-vue-next';
import { SingleOrMultipleType } from 'node_modules/radix-vue/dist/shared/types';

const { disabled = false } = defineProps<{
  disabled?: boolean;
  toggleType: SingleOrMultipleType;
}>();

const model = defineModel<string[]>();
</script>

<template>
  <ToggleGroup :type="toggleType" v-model="model" :disabled="disabled">
    <ToggleGroupItem
      :value="ExerciseMetricType.Weight.toString()"
      aria-label="Toggle reps"
      class="whitespace-pre"
    >
      <Weight class="h-4 w-4" /> Weight
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Duration.toString()"
      aria-label="Toggle duration"
      class="whitespace-pre"
    >
      <Clock class="h-4 w-4" /> Duration
    </ToggleGroupItem>
    <ToggleGroupItem
      :value="ExerciseMetricType.Distance.toString()"
      aria-label="Toggle distance"
      class="whitespace-pre"
    >
      <Route class="h-4 w-4" /> Distance
    </ToggleGroupItem>
  </ToggleGroup>
</template>
