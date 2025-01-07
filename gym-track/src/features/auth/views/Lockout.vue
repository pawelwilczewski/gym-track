<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/default-layout.vue';
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
      <Alert variant="destructive">
        <AlertCircle class="w-4 h-4" />
        <AlertTitle>Locked out</AlertTitle>
        <AlertDescription>
          This account has been locked out. Please try again later.
        </AlertDescription>
      </Alert>
    </div>
  </DefaultLayout>
</template>
