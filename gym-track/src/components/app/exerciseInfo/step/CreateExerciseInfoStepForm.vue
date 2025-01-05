<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createExerciseInfoStepSchema } from '@/app/schema/Schemas';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from './ExerciseInfoStepForm.vue';
import { useExerciseInfoSteps } from '@/app/stores/UseExerciseInfoSteps';

const { exerciseInfoId } = defineProps<{ exerciseInfoId: UUID }>();

const steps = useExerciseInfoSteps();

const form = useForm({
  validationSchema: toTypedSchema(createExerciseInfoStepSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  steps.create(exerciseInfoId, values, form);
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
