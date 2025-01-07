<script setup lang="ts">
import { useForm } from 'vee-validate';
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/features/shared/components/ui/form';
import DefaultLayout from '@/features/shared/layouts/default-layout.vue';
import Button from '@/features/shared/components/ui/button/Button.vue';
import Input from '@/features/shared/components/ui/input/Input.vue';
import Label from '@/features/shared/components/ui/label/Label.vue';
import router from '@/router';
import { Checkbox } from '@/features/shared/components/ui/checkbox';
import { logInSchema } from '@/features/auth/schemas/log-in-schema';

import { toTypedSchema } from '@vee-validate/zod';
import { useAuth } from '@/features/auth/stores/use-auth';

const auth = useAuth();

const form = useForm({
  validationSchema: toTypedSchema(logInSchema),
});

const onSubmit = form.handleSubmit(async values => {
  if (!(await auth.logIn(values))) {
    return;
  }

  router.push('/');
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
              <FormLabel class="!text-current !mt-0 !mb-0">
                Remember Me
              </FormLabel>
              <FormMessage />
            </FormItem>
          </FormField>

          <RouterLink to="/forgot-password" class="text-sm hover:underline">
            Forgot Password?
          </RouterLink>
        </div>

        <Button class="mx-auto px-8 mt-4" type="submit">Log In</Button>
      </form>

      <Label class="block text-center text-lg mt-16 font-bold">
        No account?
      </Label>
      <RouterLink to="/sign-up" class="block text-center hover:underline">
        Sign Up
      </RouterLink>
    </div>
  </DefaultLayout>
</template>
