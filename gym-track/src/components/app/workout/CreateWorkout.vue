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
import { createWorkoutSchema } from '@/scripts/schema/Schemas';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/scripts/errors/Handlers';

const form = useForm({
  validationSchema: createWorkoutSchema,
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post('/api/v1/workouts', {
    name: values.name,
  });

  if (
    !ErrorHandler.forResponse(response)
      .withPartial(formErrorHandler, form)
      .withFull(toastErrorHandler)
      .handle()
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
    <FormField v-slot="{ componentField }" name="name">
      <FormItem>
        <FormLabel class="text-lg !text-current">Name</FormLabel>
        <FormControl>
          <Input placeholder="Name" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto px-8 mt-4" type="submit">Create</Button>
  </form>
</template>
