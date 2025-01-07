<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/default-layout.vue';
import Button from '@/features/shared/components/ui/button/Button.vue';
import { useRoute } from 'vue-router';

const email = useRoute().query.email;
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center mb-16">Confirm Email</h1>
    <div class="text-center text-lg mt-16">
      Please confirm your email
      <i>{{ email }}</i>
      by clicking on the link sent to your inbox.
    </div>
    <div class="text-center text-lg mt-16">
      <div>Email not arrived?</div>
      <Button variant="link" class="font-normal">
        Resend Confirmation Email
      </Button>
    </div>
  </DefaultLayout>
</template>
