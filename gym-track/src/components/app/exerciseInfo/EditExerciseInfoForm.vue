<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import { createExerciseInfoSchema } from '@/app/schema/Schemas';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from './ExerciseInfoForm.vue';
import { UUID } from 'crypto';
import { z } from 'zod';
import { Override } from '@/app/utils/TypeUtils';

const props = defineProps<{
  id: UUID;
  initialValues:
    | Override<
        z.infer<typeof createExerciseInfoSchema>,
        {
          allowedMetricTypes: string[] | undefined;
          thumbnailImage: string | null | undefined;
        }
      >
    | undefined;
}>();

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('name', values.name);
  formData.append('description', values.description);
  formData.append('allowedMetricTypes', values.allowedMetricTypes.toString());
  if (values.thumbnailImage) {
    formData.append('thumbnailImage', values.thumbnailImage);
  }

  const response = await apiClient.put(
    `/api/v1/exerciseInfos/${props.id}`,
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

  emit('edited');
});

if (props.initialValues) {
  let values = props.initialValues;
  delete values.thumbnailImage;
  form.setValues(
    values as Override<
      z.infer<typeof createExerciseInfoSchema>,
      {
        allowedMetricTypes: string[] | undefined;
        thumbnailImage: undefined;
      }
    >,
    false
  );
}
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoForm
    submit-label="Save"
    :on-submit="onSubmit"
    :current-thumbnail-image-url="initialValues?.thumbnailImage"
  />
</template>
