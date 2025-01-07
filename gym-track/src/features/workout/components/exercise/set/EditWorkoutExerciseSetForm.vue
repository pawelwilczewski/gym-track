<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editWorkoutExerciseSetSchema } from '@/features/workout/schemas/EditWorkoutExerciseSetSchema';
import { WorkoutExerciseSetKey } from '@/features/workout/types/WorkoutTypes';
import {
  Distance,
  Duration,
  ExerciseMetricType,
  GetExerciseInfoResponse,
  Weight,
} from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { createWorkoutExerciseSetSchemaToRequest } from '@/features/workout/schemas/CreateWorkoutExerciseSetSchema';
import { useWorkoutExerciseSet } from '@/features/workout/composables/UseWorkoutExerciseSet';

const { workoutExerciseSetKey, exerciseInfo } = defineProps<{
  workoutExerciseSetKey: WorkoutExerciseSetKey;
  exerciseInfo: GetExerciseInfoResponse | null;
}>();

const { set, update } = useWorkoutExerciseSet(workoutExerciseSetKey);

const emit = defineEmits<{
  edited: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(editWorkoutExerciseSetSchema),
});

const onSubmit = form.handleSubmit(async values => {
  await update(createWorkoutExerciseSetSchemaToRequest(values), form);

  emit('edited');
});

if (set.value) {
  const base = {
    reps: set.value.reps,
    metricType: set.value.metric.$type.toString(),
    distanceValue: undefined,
    distanceUnits: undefined,
    weightValue: undefined,
    weightUnits: undefined,
    time: undefined,
  };

  switch (set.value.metric.$type) {
    case ExerciseMetricType.Weight: {
      const weight = set.value.metric as Weight;
      form.setValues(
        {
          ...base,
          weightUnits: weight.units.toString(),
          weightValue: weight.value,
        },
        false
      );
      break;
    }
    case ExerciseMetricType.Duration: {
      const duration = set.value.metric as Duration;
      form.setValues({ ...base, time: duration.time }, false);
      break;
    }
    case ExerciseMetricType.Distance: {
      const distance = set.value.metric as Distance;
      form.setValues(
        {
          ...base,
          distanceUnits: distance.units.toString(),
          distanceValue: distance.value,
        },
        false
      );
      break;
    }
    default: {
      throw Error('Unreachable code');
    }
  }
}
</script>

<template>
  <WorkoutExerciseSetForm
    :form="form"
    :exercise-info="exerciseInfo"
    submit-label="Save"
    :on-submit="onSubmit"
  />
</template>
