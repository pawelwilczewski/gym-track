<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import {
  EditWorkoutExerciseSetInitialValues,
  editWorkoutExerciseSetSchema,
} from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import {
  GetExerciseInfoResponse,
  WorkoutExerciseSetKey,
} from '@/app/schema/Types';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { createWorkoutExerciseSchemaToRequest } from '@/app/schema/Converters';

const props = defineProps<{
  workoutExerciseSetKey: WorkoutExerciseSetKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
  initialValues: EditWorkoutExerciseSetInitialValues | undefined;
}>();

const emit = defineEmits<{
  edited: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(editWorkoutExerciseSetSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const request = createWorkoutExerciseSchemaToRequest(values);

  if (!request.metric) {
    throw new Error(`Invalid request metric type: ${values.metricType}`);
  }

  const response = await apiClient.put(
    `/api/v1/workouts/${props.workoutExerciseSetKey.workoutId}/exercises/${props.workoutExerciseSetKey.exerciseIndex}/sets/${props.workoutExerciseSetKey.index}`,
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

  emit('edited');
});

if (props.initialValues) {
  form.setValues(props.initialValues, false);
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
