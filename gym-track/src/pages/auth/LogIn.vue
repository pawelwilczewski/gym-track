<script setup lang="ts">
import { useForm } from 'vee-validate';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

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
import { apiClient } from '@/scripts/http/Clients';
import { match, P } from 'ts-pattern';
import { toResult } from '@/scripts/errors/ResponseResult';
import router from '@/Router';
import { Checkbox } from '@/components/ui/checkbox';

const formSchema = toTypedSchema(
  z.object({
    email: z.string().email(),
    password: z.string().min(2, 'Password must contain at least 2 characters'),
    rememberMe: z.boolean().default(true).optional(),
  })
);

const form = useForm({
  validationSchema: formSchema,
});

const onSubmit = form.handleSubmit(async values => {
  console.log(values.rememberMe);
  await apiClient
    .post(
      `/auth/login?useCookies=true&useSessionCookies=${!values.rememberMe}`,
      {
        email: values.email,
        password: values.password,
      }
    )
    .then(response => {
      match(toResult(response))
        .with({ type: 'success' }, () => {
          console.log('Success! Response:', response);
          router.push('/');
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
  <DefaultLayout>
    <h1 class="text-center mb-16">Welcome back!</h1>

    <div class="px-8">
      <form
        class="mx-auto border border-border rounded-xl max-w-sm flex flex-col gap-6 p-8"
        @submit="onSubmit"
      >
        <FormField v-slot="{ field }" name="email">
          <FormItem>
            <FormLabel class="text-lg !text-current">Email</FormLabel>
            <FormControl>
              <Input type="email" placeholder="Email" v-bind="field" />
            </FormControl>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ field }" name="password">
          <FormItem>
            <FormLabel class="text-lg !text-current">Password</FormLabel>
            <FormControl>
              <Input type="password" placeholder="Password" v-bind="field" />
            </FormControl>
            <FormMessage />
          </FormItem>
        </FormField>

        <div class="flex flex-wrap justify-between gap-2">
          <FormField
            v-slot="{ value, handleChange }"
            type="checkbox"
            name="rememberMe"
          >
            <FormItem class="flex gap-2 items-center">
              <FormControl>
                <Checkbox :checked="value" @update:checked="handleChange" />
              </FormControl>
              <FormLabel class="!text-current !mt-0 !mb-0"
                >Remember Me</FormLabel
              >
              <FormMessage />
            </FormItem>
          </FormField>

          <a href="/forgotPassword" class="text-sm hover:underline"
            >Forgot Password?</a
          >
        </div>

        <Button class="mx-auto px-8 mt-4" type="submit">Log In</Button>
      </form>

      <Label class="block text-center text-lg mt-16 font-bold"
        >No account?</Label
      >
      <a href="/signUp" class="block text-center hover:underline">Sign Up</a>
    </div>
  </DefaultLayout>
</template>
