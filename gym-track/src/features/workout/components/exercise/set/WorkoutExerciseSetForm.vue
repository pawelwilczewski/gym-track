<script setup lang="ts">
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/features/shared/components/ui/form';
import Button from '@/features/shared/components/ui/button/Button.vue';
import {
  DistanceUnit,
  ExerciseMetricType,
  GetExerciseInfoResponse,
  WeightUnit,
} from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import ExerciseMetricTypeToggleGroup from '@/features/exerciseInfo/components/ExerciseMetricTypeToggleGroup.vue';
import Input from '@/features/shared/components/ui/input/Input.vue';
import Select from '@/features/shared/components/ui/select/Select.vue';
import SelectItem from '@/features/shared/components/ui/select/SelectItem.vue';
import SelectContent from '@/features/shared/components/ui/select/SelectContent.vue';
import SelectValue from '@/features/shared/components/ui/select/SelectValue.vue';
import SelectTrigger from '@/features/shared/components/ui/select/SelectTrigger.vue';
import { FormContext } from 'vee-validate';

defineProps<{
  form: FormContext;
  exerciseInfo: GetExerciseInfoResponse | null;
  submitLabel: string;
  onSubmit: () => void;
}>();
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="reps">
      <FormItem>
        <FormLabel class="text-lg !text-current">Reps</FormLabel>
        <FormControl>
          <Input placeholder="Reps" type="number" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField }" name="metricType">
      <FormItem>
        <FormLabel class="text-lg !text-current">Metric Type</FormLabel>
        <FormControl>
          <ExerciseMetricTypeToggleGroup
            toggle-type="single"
            v-bind="componentField"
            :enabled-options="exerciseInfo?.allowedMetricTypes"
          />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <template
      v-if="
        form.controlledValues.value.metricType == ExerciseMetricType.Distance
      "
    >
      <FormField v-slot="{ componentField }" name="distanceValue">
        <FormItem>
          <FormLabel class="text-lg !text-current">Distance</FormLabel>
          <FormControl>
            <Input
              placeholder="Distance"
              type="number"
              v-bind="componentField"
            />
          </FormControl>
          <FormMessage />
        </FormItem>
      </FormField>

      <FormField v-slot="{ componentField }" name="distanceUnits">
        <FormItem>
          <FormLabel class="text-lg !text-current">Units</FormLabel>
          <FormControl>
            <Select v-bind="componentField">
              <SelectTrigger>
                <SelectValue placeholder="Units" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem :value="DistanceUnit.Metre.toString()">
                  Metres
                </SelectItem>
                <SelectItem :value="DistanceUnit.Yard.toString()">
                  Yards
                </SelectItem>
              </SelectContent>
            </Select>
          </FormControl>
          <FormMessage />
        </FormItem>
      </FormField>
    </template>

    <template
      v-if="form.controlledValues.value.metricType == ExerciseMetricType.Weight"
    >
      <FormField v-slot="{ componentField }" name="weightValue">
        <FormItem>
          <FormLabel class="text-lg !text-current">Weight</FormLabel>
          <FormControl>
            <Input placeholder="Weight" type="number" v-bind="componentField" />
          </FormControl>
          <FormMessage />
        </FormItem>
      </FormField>

      <FormField v-slot="{ componentField }" name="weightUnits">
        <FormItem>
          <FormLabel class="text-lg !text-current">Units</FormLabel>
          <FormControl>
            <Select v-bind="componentField">
              <SelectTrigger>
                <SelectValue placeholder="Units" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem :value="WeightUnit.Kilogram.toString()">
                  Kilograms
                </SelectItem>
                <SelectItem :value="WeightUnit.Pound.toString()">
                  Pounds
                </SelectItem>
              </SelectContent>
            </Select>
          </FormControl>
          <FormMessage />
        </FormItem>
      </FormField>
    </template>

    <FormField
      v-if="
        form.controlledValues.value.metricType == ExerciseMetricType.Duration
      "
      v-slot="{ componentField }"
      name="time"
    >
      <FormItem>
        <FormLabel class="text-lg !text-current">Duration</FormLabel>
        <FormControl>
          <Input type="time" step="1" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <Button class="mx-auto mt-4" type="submit">{{ submitLabel }}</Button>
  </form>
</template>
