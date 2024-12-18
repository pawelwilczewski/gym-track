<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import { createWorkoutExerciseSetSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import {
  GetExerciseInfoResponse,
  WorkoutExerciseKey,
} from '@/app/schema/Types';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseSetForm from './WorkoutExerciseSetForm.vue';
import { createWorkoutExerciseSchemaToRequest } from '@/app/schema/Converters';

const props = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
  exerciseInfo: GetExerciseInfoResponse | undefined | null;
}>();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutExerciseSetSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const request = createWorkoutExerciseSchemaToRequest(values);

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
  <WorkoutExerciseSetForm
    :form="form"
    :exercise-info="exerciseInfo"
    submit-label="Create"
    :on-submit="onSubmit"
  />
</template>
