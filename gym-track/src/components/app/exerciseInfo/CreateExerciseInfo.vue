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
import { createExerciseInfoSchema } from '@/app/schema/Schemas';
import Textarea from '@/components/ui/textarea/Textarea.vue';
import { formErrorHandler, toastErrorHandler } from '@/app/errors/Handlers';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import ExerciseMetricTypeToggleGroup from './ExerciseMetricTypeToggleGroup.vue';

const form = useForm({
  validationSchema: createExerciseInfoSchema,
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('name', values.name);
  formData.append('description', values.description);
  formData.append('allowedMetricTypes', values.allowedMetricTypes.toString());
  if (values.thumbnailImage) {
    formData.append('thumbnailImage', values.thumbnailImage);
  }

  const response = await apiClient.post('/api/v1/exerciseInfos', formData);
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
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
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

    <FormField v-slot="{ componentField }" name="description">
      <FormItem>
        <FormLabel class="text-lg !text-current">Description</FormLabel>
        <FormControl>
          <Textarea placeholder="Description" v-bind="componentField" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField }" name="allowedMetricTypes">
      <FormItem>
        <FormLabel class="text-lg !text-current">
          Allowed Metric Types
        </FormLabel>
        <FormControl>
          <ExerciseMetricTypeToggleGroup
            toggle-type="multiple"
            v-bind="componentField"
          />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ setValue }" name="thumbnailImage">
      <FormItem>
        <FormLabel class="text-lg !text-current">Thumbnail Image</FormLabel>
        <FormControl>
          <Input
            type="file"
            accept="image/jpeg,image/png,image/gif"
            :onchange="
              (event: Event) => {
                const input = event?.currentTarget as HTMLInputElement;
                setValue(input?.files?.[0]);
              }
            "
          />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>
    <Button class="mx-auto px-8 mt-4" type="submit">Create</Button>
  </form>
</template>
