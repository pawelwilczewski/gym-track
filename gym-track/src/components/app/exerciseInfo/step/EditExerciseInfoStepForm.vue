<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from './ExerciseInfoStepForm.vue';
import {
  createExerciseInfoStepSchema,
  editExerciseInfoStepSchema,
} from '@/app/schema/Schemas';
import { ExerciseInfoStepKey } from '@/app/schema/Types';
import { Override } from '@/app/utils/TypeUtils';
import { z } from 'zod';

const props = defineProps<{
  stepKey: ExerciseInfoStepKey;
  initialValues:
    | Override<
        z.infer<typeof createExerciseInfoStepSchema>,
        {
          image: string | null | undefined;
        }
      >
    | undefined;
}>();

const form = useForm({
  validationSchema: toTypedSchema(editExerciseInfoStepSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('_method', 'PUT');
  formData.append('description', values.description);
  formData.append('replaceImage', values.replaceImage.toString());
  if (values.image) {
    formData.append('image', values.image);
  }

  const response = await apiClient.put(
    `/api/v1/exerciseInfos/${props.stepKey.exerciseInfoId}/steps/${props.stepKey.index}`,
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
  delete values.image;
  form.setValues(
    values as Override<
      z.infer<typeof createExerciseInfoStepSchema>,
      {
        thumbnailImage: undefined;
      }
    >,
    false
  );
}
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoStepForm
    :form="form"
    :image-section="{ type: 'replace', currentImageUrl: initialValues?.image }"
    submit-label="Save"
    :on-submit="onSubmit"
  />
</template>
