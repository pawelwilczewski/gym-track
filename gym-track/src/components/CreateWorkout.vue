<script setup lang="ts">
import { useForm } from 'vee-validate';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';
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

const formSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(1),
  })
);

const form = useForm({
  validationSchema: formSchema,
});

const onSubmit = form.handleSubmit(async values => {
  await apiClient
    .post('/api/v1/workouts', {
      name: values.name,
    })
    .then(response => {
      match(toResult(response))
        .with({ type: 'success' }, () => {
          console.log('Success creating new workout');
        })
        .with({ type: 'empty' }, () =>
          console.log('Unknown error encountered.')
        )
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
</script>

<template>
  <form
    class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-6 p-8"
    @submit="onSubmit"
  >
    <h2>New Workout</h2>
    <FormField v-slot="{ field }" name="name">
      <FormItem>
        <FormLabel class="text-lg !text-current">Name</FormLabel>
        <FormControl>
          <Input placeholder="Name" v-bind="field" />
        </FormControl>
        <FormMessage />
      </FormItem>
    </FormField>

    <Button class="mx-auto px-8 mt-4" type="submit">Create</Button>
  </form>
</template>
