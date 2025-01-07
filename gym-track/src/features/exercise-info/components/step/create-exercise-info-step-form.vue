<script setup lang="ts">
import { useForm } from 'vee-validate';
import { createExerciseInfoStepSchema } from '@/features/exercise-info/schemas/create-exercise-info-step-schema';
import { UUID } from 'node:crypto';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from '@/features/exercise-info/components/step/exercise-info-step-form.vue';
import { useExerciseInfoSteps } from '@/features/exercise-info/stores/use-exercise-info-steps';

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
