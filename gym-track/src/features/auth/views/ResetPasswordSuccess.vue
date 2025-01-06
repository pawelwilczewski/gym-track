<script setup lang="ts">
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Info } from 'lucide-vue-next';
</script>

<template>
  <DefaultLayout>
    <div class="p-8 max-w-lg mx-auto">
      <h1 class="mb-16 text-center">Password Reset</h1>
      <Alert>
        <Info class="h-4 w-4" />
        <AlertTitle>Success</AlertTitle>
        <AlertDescription>
          Your password has been reset. You can now return to
          <RouterLink to="/logIn">the log in page</RouterLink>.
        </AlertDescription>
      </Alert>
    </div>
  </DefaultLayout>
</template>
