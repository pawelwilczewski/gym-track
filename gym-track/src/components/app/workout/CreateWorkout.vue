<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';

import { createWorkoutSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutForm from './WorkoutForm.vue';

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const handleSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post('/api/v1/workouts', values);

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
  <WorkoutForm :on-submit="handleSubmit" submit-label="Create" />
</template>
