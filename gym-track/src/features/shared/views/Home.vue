<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/DefaultLayout.vue';
import { Button } from '@/components/ui/button';

const test = (): void => alert('hello');
</script>

<template>
  <DefaultLayout>
    <Button @click="test">Click!</Button>
  </DefaultLayout>
</template>
