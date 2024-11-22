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
import { toResult } from '@/scripts/errors/ResponseResult';
import { match, P } from 'ts-pattern';
import { createExerciseInfoSchema } from '@/scripts/schema/Schemas';
import Textarea from '@/components/ui/textarea/Textarea.vue';

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
  formData.append('thumbnailImage', values.thumbnailImage);

  await apiClient.post('/api/v1/exerciseInfos', formData).then(response => {
    match(toResult(response))
      .with({ type: 'success' }, () => {
        emit('created');
        console.log('Success creating new workout');
      })
      .with({ type: 'empty' }, () => console.log('Unknown error encountered.'))
      .with({ type: 'message', message: P.select() }, message =>
        console.log(message)
      )
      .with({ type: 'validation', errors: P.select() }, errors => {
        errors.forEach(error => {
          console.log(error);
        });
      })
      .exhaustive();
  });
});
// TODO Pawel: antiforgery tokens! here and everywhere else in forms!
</script>

<template>
  <form
    class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-6 p-8"
    @submit="onSubmit"
  >
    <h2>New Exercise</h2>
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
        <FormLabel class="text-lg !text-current"
          >Allowed Metric Types</FormLabel
        >
        <FormControl>
          <Input v-bind="componentField" type="number" />
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
