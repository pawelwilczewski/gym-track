<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/default-layout.vue';
import { Button } from '@/features/shared/components/ui/button';

const test = (): void => alert('hello');
</script>

<template>
  <DefaultLayout>
    <Button @click="test">Click!</Button>
  </DefaultLayout>
</template>
