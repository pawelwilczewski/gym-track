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
import { signUpSchema } from '@/features/auth/schemas/sign-up-schema';

import router from '@/router';
import { toTypedSchema } from '@vee-validate/zod';
import { useAuth } from '@/features/auth/stores/use-auth';

const auth = useAuth();

const form = useForm({
  validationSchema: toTypedSchema(signUpSchema),
});

const onSubmit = form.handleSubmit(async values => {
  if (!(await auth.signUp(values, form))) {
    return;
  }

  router.push(`/confirm-email?email=${values.email}`);
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
            <FormLabel class="text-lg !text-current">
              Confirm Password
            </FormLabel>
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

      <Label class="block text-center text-lg mt-16 font-bold">
        Already have an Account?
      </Label>
      <RouterLink to="/log-in" class="block text-center hover:underline">
        Log In
      </RouterLink>
    </div>
  </DefaultLayout>
</template>
