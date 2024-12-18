<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import Button from '@/components/ui/button/Button.vue';
import { createWorkoutExerciseSetSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import {
  DistanceUnit,
  ExerciseMetric,
  ExerciseMetricType,
  GetExerciseInfoResponse,
  WeightUnit,
  WorkoutExerciseKey,
} from '@/app/schema/Types';
import ExerciseMetricTypeToggleGroup from '@/components/app/exerciseInfo/ExerciseMetricTypeToggleGroup.vue';
import Input from '@/components/ui/input/Input.vue';
import Select from '@/components/ui/select/Select.vue';
import SelectItem from '@/components/ui/select/SelectItem.vue';
import SelectContent from '@/components/ui/select/SelectContent.vue';
import SelectValue from '@/components/ui/select/SelectValue.vue';
import SelectTrigger from '@/components/ui/select/SelectTrigger.vue';

const props = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
}>();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: createWorkoutExerciseSetSchema,
});

const onSubmit = form.handleSubmit(async values => {
  const request: { reps: number; metric: ExerciseMetric | undefined } = {
    reps: values.reps,
    metric:
      values.metricType === ExerciseMetricType.Distance
        ? {
            $type: 'Distance',
            value: values.distanceValue!,
            units: values.distanceUnits!,
          }
        : values.metricType === ExerciseMetricType.Duration
          ? {
              $type: 'Duration',
              time: values.time!,
            }
          : values.metricType === ExerciseMetricType.Weight
            ? {
                $type: 'Weight',
                value: values.weightValue!,
                units: values.weightUnits!,
              }
            : undefined,
  };

  if (!request.metric) {
    throw new Error(`Invalid request metric type: ${values.metricType}`);
  }

  const response = await apiClient.post(
    `/api/v1/workouts/${props.workoutExerciseKey.workoutId}/exercises/${props.workoutExerciseKey.index}/sets`,
    request
  );

  if (
    ErrorHandler.forResponse(response)
      .handlePartially(formErrorHandler, form)
      .handleFully(toastErrorHandler)
      .wasError()
  ) {
    return;
  }

  emit('created');
});
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
      <FormField
        v-if="
          form.controlledValues.value.metricType == ExerciseMetricType.Weight
        "
        v-slot="{ componentField }"
        name="weightValue"
      >
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

    <template
      v-if="
        form.controlledValues.value.metricType == ExerciseMetricType.Duration
      "
    >
      <FormField v-slot="{ componentField }" name="time">
        <FormItem>
          <FormLabel class="text-lg !text-current">Duration</FormLabel>
          <FormControl>
            <Input type="time" step="1" v-bind="componentField" />
          </FormControl>
          <FormMessage />
        </FormItem>
      </FormField>
    </template>

    <Button class="mx-auto mt-4" type="submit">Create</Button>
  </form>
</template>
