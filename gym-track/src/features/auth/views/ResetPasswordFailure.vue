<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/DefaultLayout.vue';
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '@/features/shared/components/ui/alert';
import { AlertCircle } from 'lucide-vue-next';
</script>

<template>
  <DefaultLayout>
    <div class="p-8 max-w-lg mx-auto">
      <h1 class="mb-16 text-center">Password Reset</h1>
      <Alert variant="destructive">
        <AlertCircle class="w-4 h-4" />
        <AlertTitle>Error</AlertTitle>
        <AlertDescription>Invalid password reset link.</AlertDescription>
      </Alert>
    </div>
  </DefaultLayout>
</template>
