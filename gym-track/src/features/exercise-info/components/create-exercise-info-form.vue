<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createExerciseInfoSchema } from '@/features/exercise-info/schemas/create-exercise-info-schema';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from '@/features/exercise-info/components/exercise-info-form.vue';
import { useExerciseInfos } from '@/features/exercise-info/stores/use-exercise-infos';

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoSchema),
});
// computed(() => form.controlledValues.value.thumbnailImage != null);
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
