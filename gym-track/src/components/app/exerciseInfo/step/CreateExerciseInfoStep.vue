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
import { createExerciseInfoStepSchema } from '@/scripts/schema/Schemas';
import Textarea from '@/components/ui/textarea/Textarea.vue';
import { formErrorHandler, toastErrorHandler } from '@/scripts/errors/Handlers';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import { UUID } from 'crypto';

const props = defineProps<{ exerciseInfoId: UUID }>();

const form = useForm({
  validationSchema: createExerciseInfoStepSchema,
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  const formData = new FormData();
  formData.append('index', values.index.toString());
  formData.append('description', values.description);
  if (values.image) {
    formData.append('image', values.image);
  }

  const response = await apiClient.post(
    `/api/v1/exerciseInfos/${props.exerciseInfoId}/steps`,
    formData
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
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <form class="flex flex-col gap-6 mt-6" @submit="onSubmit">
    <FormField v-slot="{ componentField }" name="index">
      <FormItem>
        <FormLabel class="text-lg !text-current">Index</FormLabel>
        <FormControl>
          <Input placeholder="Index" type="number" v-bind="componentField" />
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
    <FormField v-slot="{ setValue }" name="image">
      <FormItem>
        <FormLabel class="text-lg !text-current">Image</FormLabel>
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
