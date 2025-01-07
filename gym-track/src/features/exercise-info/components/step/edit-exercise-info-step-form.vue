<script setup lang="ts">
import { useForm } from 'vee-validate';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from '@/features/exercise-info/components/step/exercise-info-step-form.vue';
import { editExerciseInfoStepSchema } from '@/features/exercise-info/schemas/edit-exercise-info-step-schema';
import { ExerciseInfoStepKey } from '@/features/exercise-info/types/exercise-info-types';
import { useExerciseInfoStep } from '@/features/exercise-info/composables/use-exercise-info-step';
import { apiClient } from '@/features/shared/http/api-client';

const { stepKey } = defineProps<{ stepKey: ExerciseInfoStepKey }>();

const { step, update } = useExerciseInfoStep(stepKey);

const form = useForm({
  validationSchema: toTypedSchema(editExerciseInfoStepSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  update(values, form);
  emit('edited');
});

if (step.value) {
  form.setValues({ description: step.value.description }, false);
}
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <ExerciseInfoStepForm
    :form="form"
    :image-section="{
      type: 'replace',
      currentImageUrl:
        step?.imageUrl != null
          ? `${apiClient.getUri()}/${step.imageUrl}`
          : null,
    }"
    submit-label="Save"
    :on-submit="onSubmit"
  />
</template>
