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
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import Label from '@/components/ui/label/Label.vue';
import { signUpSchema } from '@/scripts/schema/Schemas';
import { formErrorHandler, toastErrorHandler } from '@/scripts/errors/Handlers';
import { ErrorHandler } from '@/scripts/errors/ErrorHandler';
import router from '@/Router';

const form = useForm({
  validationSchema: signUpSchema,
});

const onSubmit = form.handleSubmit(async values => {
  const response = await apiClient.post('/auth/register', {
    email: values.email,
    password: values.password,
  });

  if (
    !ErrorHandler.forResponse(response)
      .with(formErrorHandler, form)
      .with(toastErrorHandler)
      .handle()
  ) {
    return;
  }

  router.push(`/confirmEmail?email=${values.email}`);
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
      <RouterLink to="/logIn" class="block text-center hover:underline"
        >Log In</RouterLink
      >
    </div>
  </DefaultLayout>
</template>
