<script setup lang="ts">
import { useForm } from 'vee-validate';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';
import { apiClient } from '@/scripts/Http/Clients';

import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import Label from '@/components/ui/label/Label.vue';
import { toResult } from '@/scripts/ErrorHandling/ResponseResult';
import { match, P } from 'ts-pattern';
import router from '@/router';

const formSchema = toTypedSchema(
  z
    .object({
      email: z.string().email(),
      password: z
        .string()
        .min(2, 'Password must contain at least 2 characters'),
      confirmPassword: z
        .string()
        .min(2, 'Password must contain at least 2 characters'),
    })
    .superRefine(({ confirmPassword, password }, ctx) => {
      if (confirmPassword !== password) {
        ctx.addIssue({
          code: 'custom',
          message: 'The passwords do not match',
          path: ['confirmPassword'],
        });
      }
    })
);

const form = useForm({
  validationSchema: formSchema,
});

const onSubmit = form.handleSubmit(async (values) => {
  await apiClient
    .post('/auth/register', { email: values.email, password: values.password })
    .then((response) => {
      match(toResult(response))
        .with({ type: 'success' }, () => {
          console.log('Success!');
          router.push('/confirmEmail');
        })
        .with({ type: 'empty' }, () =>
          console.log('Unknown error encountered.')
        )
        .with({ type: 'message', message: P.select() }, (message) =>
          console.log(message)
        )
        .with({ type: 'validation', errors: P.select() }, (errors) => {
          errors.forEach((error) => {
            console.log(error);
          });
        })
        .exhaustive();
    });
});
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center mb-16">Welcome!</h1>

    <div class="px-8">
      <form
        class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-6 p-8"
        @submit="onSubmit"
      >
        <FormField v-slot="{ componentField }" name="email">
          <FormItem>
            <FormLabel class="text-lg !text-current">Email</FormLabel>
            <FormControl>
              <Input type="email" placeholder="Email" v-bind="componentField" />
            </FormControl>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ componentField }" name="password">
          <FormItem>
            <FormLabel class="text-lg !text-current">Password</FormLabel>
            <FormControl>
              <Input
                type="password"
                placeholder="Password"
                v-bind="componentField"
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ componentField }" name="confirmPassword">
          <FormItem>
            <FormLabel class="text-lg !text-current"
              >Confirm Password</FormLabel
            >
            <FormControl>
              <Input
                type="password"
                placeholder="Password"
                v-bind="componentField"
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        </FormField>

        <Button class="mx-auto px-8 mt-4" type="submit">Sign Up</Button>
      </form>

      <Label class="block text-center text-lg mt-16 font-bold"
        >Already have an Account?</Label
      >
      <a href="/logIn" class="block text-center hover:underline">Log In</a>
    </div>
  </DefaultLayout>
</template>
