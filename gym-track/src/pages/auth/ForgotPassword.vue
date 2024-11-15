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
import FormDescription from '@/components/ui/form/FormDescription.vue';
import { Mail } from 'lucide-vue-next';

import { Ref, ref } from 'vue';
import Countdown from '@/components/Countdown.vue';

const formSchema = toTypedSchema(
  z.object({
    email: z.string().email(),
  })
);

const form = useForm({
  validationSchema: formSchema,
});

const resubmitTime: number = 60;

const countdown: Ref<typeof Countdown | null> = ref(null);
const sendEmailEnabled = ref(true);

const onSubmit = form.handleSubmit((values) => {
  if (!sendEmailEnabled) {
    return;
  }

  console.log('Resetting password!', values);

  countdown.value!.start();
  sendEmailEnabled.value = false;
});

const handleCountdownComplete: () => void = () => {
  sendEmailEnabled.value = true;
};
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center text-5xl font-bold mb-16">Reset Password</h1>

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
            <FormDescription
              >If an account with this email exists, the reset link will be
              sent.</FormDescription
            >
            <FormMessage />
          </FormItem>
        </FormField>

        <div class="flex gap-2 justify-center mt-4">
          <Button :disabled="!sendEmailEnabled" type="submit"
            ><Mail class="w-4 h-4 mr-2" /> Send Reset Email</Button
          >
          <div v-show="!sendEmailEnabled" class="my-auto">
            <Countdown
              :total-duration-seconds="resubmitTime"
              :tick-interval-seconds="0.1"
              @complete="handleCountdownComplete"
              ref="countdown"
            ></Countdown>
          </div>
        </div>
      </form>
    </div>
  </DefaultLayout>
</template>
