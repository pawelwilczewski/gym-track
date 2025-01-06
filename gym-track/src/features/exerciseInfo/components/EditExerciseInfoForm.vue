<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editExerciseInfoSchema } from '@/features/exerciseInfo/schemas/EditExerciseInfoSchema';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from '@/features/exerciseInfo/components/ExerciseInfoForm.vue';
import { UUID } from 'crypto';
import { useExerciseInfo } from '@/features/exerciseInfo/composables/UseExerciseInfo';
import { enumFlagsValueToStringArray } from '@/features/shared/utils/ZodUtils';
import { apiClient } from '@/features/shared/http/ApiClient';

const props = defineProps<{
  id: UUID;
}>();

const { exerciseInfo, update } = useExerciseInfo(props.id);

const form = useForm({
  validationSchema: toTypedSchema(editExerciseInfoSchema),
});

const emit = defineEmits<{
  edited: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  await update(values, form);
  emit('edited');
});

if (exerciseInfo.value) {
  form.setValues(
    {
      description: exerciseInfo.value.description,
      allowedMetricTypes: enumFlagsValueToStringArray(
        exerciseInfo.value.allowedMetricTypes
      ),
      name: exerciseInfo.value.name,
    },
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
      currentThumbnailImageUrl:
        exerciseInfo?.thumbnailUrl != null
          ? `${apiClient.getUri()}/${exerciseInfo.thumbnailUrl}`
          : null,
    }"
    :on-submit="onSubmit"
    submit-label="Save"
  />
</template>
