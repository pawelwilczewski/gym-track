<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createExerciseInfoSchema } from '@/app/schema/Schemas';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from './ExerciseInfoForm.vue';
import { useExerciseInfos } from '@/app/stores/UseExerciseInfos';

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoSchema),
});

const exerciseInfos = useExerciseInfos();

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  await exerciseInfos.create(values, form);
  emit('created');
});
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoForm
    :form="form"
    :thumbnail-image-section="{ type: 'upload' }"
    submit-label="Create"
    :on-submit="onSubmit"
  />
</template>
