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
import Input from '@/components/ui/input/Input.vue';
import { createWorkoutExerciseSchema } from '@/scripts/schema/Schemas';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/scripts/errors/Handlers';
import { GetWorkoutResponse } from '@/scripts/schema/Types';

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
  const index =
    props.workout.exercises.length > 0
      ? Math.max(...props.workout.exercises.map(key => key.index)) + 1
      : 0;
  const response = await apiClient.post(
    `/api/v1/workouts/${props.workout.id}/exercises`,
    {
      index: index,
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
  <form
    class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-6 p-8"
    @submit="onSubmit"
  >
    <h2>New Workout</h2>
    <FormField v-slot="{ componentField }" name="exerciseInfoId">
      <FormItem>
        <FormLabel class="text-lg !text-current">Exercise Info</FormLabel>
        <FormControl>
          <Input v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto px-8 mt-4" type="submit">Create</Button>
  </form>
</template>
