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
import FormDescription from '@/features/shared/components/ui/form/FormDescription.vue';
import { Mail } from 'lucide-vue-next';
import { ref } from 'vue';
import Countdown from '@/features/shared/components/countdown.vue';
import { forgotPasswordSchema } from '@/features/auth/schemas/forgot-password-schema';
import { toTypedSchema } from '@vee-validate/zod';

const form = useForm({
  validationSchema: toTypedSchema(forgotPasswordSchema),
});

const resubmitTime: number = 60;

const countdown = ref<typeof Countdown | undefined>(undefined);
const sendEmailEnabled = ref(true);

const onSubmit = form.handleSubmit(values => {
  if (!sendEmailEnabled.value) {
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
    <h1 class="text-center mb-16">Reset Password</h1>

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
            <FormDescription>
              If an account with this email exists, the reset link will be
            </FormDescription>
            <FormMessage />
          </FormItem>
        </FormField>

        <div class="flex gap-2 justify-center mt-4">
          <Button :disabled="!sendEmailEnabled" type="submit">
            <Mail class="w-4 h-4 mr-2" />
            Send Reset Email
          </Button>
          <div v-show="!sendEmailEnabled" class="my-auto">
            <Countdown
              ref="countdown"
              :total-duration-seconds="resubmitTime"
              :tick-interval-seconds="1"
              @complete="handleCountdownComplete"
            ></Countdown>
          </div>
        </div>
      </form>
    </div>
  </DefaultLayout>
</template>
