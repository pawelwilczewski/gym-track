<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';

import { createExerciseInfoSchema } from '@/app/schema/Schemas';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from './ExerciseInfoForm.vue';

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('name', values.name);
  formData.append('description', values.description);
  formData.append('allowedMetricTypes', values.allowedMetricTypes.toString());
  if (values.thumbnailImage) {
    formData.append('thumbnailImage', values.thumbnailImage);
  }

  const response = await apiClient.post('/api/v1/exerciseInfos', formData);
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
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoForm
    :form="form"
    thumbnail-image-section="upload"
    submit-label="Create"
    :on-submit="onSubmit"
  />
</template>
