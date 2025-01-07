<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editWorkoutExerciseSetSchema } from '@/features/workout/schemas/edit-workout-exercise-set-schema';
import { WorkoutExerciseSetKey } from '@/features/workout/types/workout-types';
import {
  Distance,
  Duration,
  ExerciseMetricType,
  GetExerciseInfoResponse,
  Weight,
} from '@/features/exercise-info/types/exercise-info-types';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './workout-exercise-set-form.vue';
import { createWorkoutExerciseSetSchemaToRequest } from '@/features/workout/schemas/create-workout-exercise-set-schema';
import { useWorkoutExerciseSet } from '@/features/workout/composables/use-workout-exercise-set';

const { workoutExerciseSetKey, exerciseInfo } = defineProps<{
  workoutExerciseSetKey: WorkoutExerciseSetKey;
  exerciseInfo: GetExerciseInfoResponse | undefined;
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
      throw new Error('Unreachable code');
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
