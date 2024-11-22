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
import { createWorkoutSchema } from '@/scripts/schema/Schemas';

const form = useForm({
  validationSchema: createWorkoutSchema,
});

const emit = defineEmits<{
  created: [];
}>();

const onSubmit = form.handleSubmit(async values => {
  await apiClient
    .post('/api/v1/workouts', {
      name: values.name,
    })
    .then(response => {
      match(toResult(response))
        .with({ type: 'success' }, () => {
          emit('created');
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
