<script setup lang="ts">
import { useForm } from 'vee-validate';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoStepForm from '@/features/exerciseInfo/components/step/ExerciseInfoStepForm.vue';
import { editExerciseInfoStepSchema } from '@/features/exerciseInfo/schemas/EditExerciseInfoStepSchema';
import { ExerciseInfoStepKey } from '@/features/exerciseInfo/types/ExerciseInfoTypes';
import { useExerciseInfoStep } from '@/features/exerciseInfo/composables/UseExerciseInfoStep';
import { apiClient } from '@/features/shared/http/ApiClient';

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
