<script setup lang="ts">
import { useForm } from 'vee-validate';
import { apiClient } from '@/scripts/http/Clients';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import Button from '@/components/ui/button/Button.vue';
import { createWorkoutExerciseSchema } from '@/scripts/schema/Schemas';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/scripts/errors/Handlers';
import { GetWorkoutResponse } from '@/scripts/schema/Types';
import ExerciseInfosDropdown from '../../exerciseInfo/ExerciseInfosCombobox.vue';

const props = defineProps<{
  workout: GetWorkoutResponse;
}>();

const emit = defineEmits<{
  created: [];
}>();

const form = useForm({
  validationSchema: createWorkoutExerciseSchema,
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post(
    `/api/v1/workouts/${props.workout.id}/exercises`,
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
