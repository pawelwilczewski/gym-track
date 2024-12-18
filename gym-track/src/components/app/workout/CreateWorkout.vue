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
import Input from '@/components/ui/input/Input.vue';
import { createWorkoutSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { toTypedSchema } from '@vee-validate/zod';

const form = useForm({
  validationSchema: toTypedSchema(createWorkoutSchema),
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post('/api/v1/workouts', {
    name: values.name,
  });

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
