<script setup lang="ts">
import { useForm } from 'vee-validate';
import { editExerciseInfoSchema } from '@/features/exercise-info/schemas/edit-exercise-info-schema';
import { toTypedSchema } from '@vee-validate/zod';
import ExerciseInfoForm from '@/features/exercise-info/components/exercise-info-form.vue';
import { UUID } from 'node:crypto';
import { useExerciseInfo } from '@/features/exercise-info/composables/use-exercise-info';
import { enumFlagsValueToStringArray } from '@/features/shared/utils/zod-utils';
import { apiClient } from '@/features/shared/http/api-client';

const properties = defineProps<{
  id: UUID;
}>();

const { exerciseInfo, update } = useExerciseInfo(properties.id);

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
          : undefined,
    }"
    :on-submit="onSubmit"
    submit-label="Save"
  />
</template>
