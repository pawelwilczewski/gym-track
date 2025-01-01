<script setup lang="ts">
import { useForm } from 'vee-validate';
import {
  createExerciseInfoSchema,
  editExerciseInfoSchema,
} from '@/app/schema/Schemas';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from './ExerciseInfoForm.vue';
import { UUID } from 'crypto';
import { z } from 'zod';
import { Override } from '@/app/utils/TypeUtils';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

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

const exerciseInfos = useExerciseInfos();

const form = useForm({
  validationSchema: toTypedSchema(editExerciseInfoSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  await exerciseInfos.update(props.id, values, form);
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
    :form="form"
    :thumbnail-image-section="{
      type: 'replace',
      currentThumbnailImageUrl: initialValues?.thumbnailImage,
    }"
    :on-submit="onSubmit"
    submit-label="Save"
  />
</template>
