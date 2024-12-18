<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';

import { createWorkoutExerciseSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseForm from '@/components/app/workout/exercise/WorkoutExerciseForm.vue';

const props = defineProps<{
  workoutId: UUID;
}>();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutExerciseSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post(
    `/api/v1/workouts/${props.workoutId}/exercises`,
    {
      exerciseInfoId: values.exerciseInfoId,
    }
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
  <WorkoutExerciseForm :on-submit="onSubmit" submit-label="Create" />
</template>
