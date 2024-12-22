<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import { createExerciseInfoStepSchema } from '@/app/schema/Schemas';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from './ExerciseInfoStepForm.vue';

const props = defineProps<{ exerciseInfoId: UUID }>();

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoStepSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('description', values.description);
  if (values.image) {
    formData.append('image', values.image);
  }

  const response = await apiClient.post(
    `/api/v1/exerciseInfos/${props.exerciseInfoId}/steps`,
    formData
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
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoStepForm
    :form="form"
    :image-section="{ type: 'upload' }"
    submit-label="Create"
    :on-submit="onSubmit"
  />
</template>
