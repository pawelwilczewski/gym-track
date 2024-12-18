<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';

import { createWorkoutSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { toTypedSchema } from '@vee-validate/zod';
import { z } from 'zod';
import { UUID } from 'crypto';
import WorkoutForm from './WorkoutForm.vue';

const props = defineProps<{
  workoutId: UUID;
  initialValues: z.infer<typeof createWorkoutSchema>;
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const handleSubmit = form.handleSubmit(async values => {
  const response = await apiClient.put(
    `/api/v1/workouts/${props.workoutId}`,
    values
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
  <WorkoutForm submit-label="Save" :on-submit="handleSubmit" />
</template>
