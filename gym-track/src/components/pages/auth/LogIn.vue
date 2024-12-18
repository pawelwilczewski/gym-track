<script setup lang="ts">
import { useForm } from 'vee-validate';
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
import { apiClient } from '@/app/http/Clients';
import router from '@/Router';
import { Checkbox } from '@/components/ui/checkbox';
import { logInRequestSchema } from '@/app/schema/Schemas';
import { ErrorHandler } from '@/app/errors/ErrorHandler';
import {
  invalidCredentialsErrorHandler,
  toastErrorHandler,
} from '@/app/errors/Handlers';
import { toTypedSchema } from '@vee-validate/zod';

const form = useForm({
  validationSchema: toTypedSchema(logInRequestSchema),
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post(
    `/auth/login?useCookies=true&useSessionCookies=${!values.rememberMe}`,
    {
      email: values.email,
      password: values.password,
    }
  );

  if (
    ErrorHandler.forResponse(response)
      .handlePartially(invalidCredentialsErrorHandler)
      .handleFully(toastErrorHandler)
      .wasError()
  ) {
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

          <RouterLink to="/forgotPassword" class="text-sm hover:underline">
            Forgot Password?
          </RouterLink>
        </div>

        <Button class="mx-auto px-8 mt-4" type="submit">Log In</Button>
      </form>

      <Label class="block text-center text-lg mt-16 font-bold">
        No account?
      </Label>
      <RouterLink to="/signUp" class="block text-center hover:underline">
        Sign Up
      </RouterLink>
    </div>
  </DefaultLayout>
</template>
