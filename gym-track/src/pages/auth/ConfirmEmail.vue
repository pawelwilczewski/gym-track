<script setup lang="ts">
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import Button from '@/components/ui/button/Button.vue';
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center mb-16">Confirm Email</h1>
    <div class="text-center text-lg mt-16">
      Please confirm your email by clicking on the link sent to your inbox.
    </div>
    <div class="text-center text-lg mt-16">
      <div>Email not arrived?</div>
      <Button variant="link" class="font-normal"
        >Resend Confirmation Email</Button
      >
    </div>
  </DefaultLayout>
</template>
