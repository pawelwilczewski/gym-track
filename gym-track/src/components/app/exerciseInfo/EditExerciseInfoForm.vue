<script setup lang="ts">
import { useForm } from 'vee-validate';
import {
  EditExerciseInfoInitialValues,
  editExerciseInfoSchema,
  EditExerciseInfoValues,
} from '@/app/schema/Schemas';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from './ExerciseInfoForm.vue';
import { UUID } from 'crypto';
import { useExerciseInfo } from '@/composables/UseExerciseInfo';

const props = defineProps<{
  id: UUID;
  initialValues: EditExerciseInfoInitialValues | undefined;
}>();

const exerciseInfo = useExerciseInfo(props.id);

const form = useForm({
  validationSchema: toTypedSchema(editExerciseInfoSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  await exerciseInfo.update(values, form);
  emit('edited');
});

if (props.initialValues) {
  let values = props.initialValues;
  delete values.thumbnailImage;
  form.setValues(values as EditExerciseInfoValues, false);
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
