<script setup lang="ts">
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { AlertCircle, Info } from 'lucide-vue-next';
import { ref } from 'vue';

const success = ref(true);
</script>

<template>
  <DefaultLayout>
    <div class="p-8 max-w-lg mx-auto">
      <h1 class="mb-16 text-center">Password Reset</h1>

      <Alert v-if="success">
        <Info class="h-4 w-4" />
        <AlertTitle>Success</AlertTitle>
        <AlertDescription>
          Your password has been reset. You can now return to
          <a href="/logIn">the log in page</a>.
        </AlertDescription>
      </Alert>

      <Alert v-else variant="destructive">
        <AlertCircle class="w-4 h-4" />
        <AlertTitle>Error</AlertTitle>
        <AlertDescription> Invalid password reset link. </AlertDescription>
      </Alert>
    </div>
  </DefaultLayout>
</template>
