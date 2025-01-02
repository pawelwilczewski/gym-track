<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';

import { editWorkoutExerciseSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { toTypedSchema } from '@vee-validate/zod';
import WorkoutExerciseForm from '@/components/app/workout/exercise/WorkoutExerciseForm.vue';
import { WorkoutExerciseKey } from '@/app/schema/Types';
import { z } from 'zod';

const props = defineProps<{
  workoutExerciseKey: WorkoutExerciseKey;
  initialValues: z.infer<typeof editWorkoutExerciseSchema>;
}>();

const emit = defineEmits<{
  edited: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(editWorkoutExerciseSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.put(
    `/api/v1/workouts/${props.workoutExerciseKey.workoutId}/exercises/${props.workoutExerciseKey.index}`,
    values
  );

  if (
    ErrorHandler.forResponse(response)
      .handlePartially(formErrorHandler, form)
      .handleFully(toastErrorHandler)
      .wasError()
  ) {
    return;
  }

  emit('edited');
});

if (props.initialValues) {
  form.setValues(props.initialValues, false);
}
</script>

<template>
  <WorkoutExerciseForm :on-submit="onSubmit" submit-label="Save" />
</template>
