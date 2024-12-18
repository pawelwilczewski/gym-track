<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/app/http/Clients';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import Button from '@/components/ui/button/Button.vue';
import { createWorkoutExerciseSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import ExerciseInfosDropdown from '../../exerciseInfo/ExerciseInfosCombobox.vue';
import { UUID } from 'crypto';
import { toTypedSchema } from '@vee-validate/zod';

const props = defineProps<{
  workoutId: UUID;
}>();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutExerciseSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post(
    `/api/v1/workouts/${props.workoutId}/exercises`,
    {
      exerciseInfoId: values.exerciseInfoId,
    }
  );

  if (
    ErrorHandler.forResponse(response)
      .handlePartially(formErrorHandler, form)
      .handleFully(toastErrorHandler)
      .wasError()
  ) {
    return;
  }

  emit('created');
});
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="exerciseInfoId">
      <FormItem>
        <FormLabel class="text-lg !text-current">Exercise</FormLabel>
        <FormControl>
          <ExerciseInfosDropdown v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto mt-4" type="submit">Create</Button>
  </form>
</template>
