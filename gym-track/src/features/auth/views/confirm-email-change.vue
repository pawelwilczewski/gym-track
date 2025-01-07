<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/default-layout.vue';
import { ref } from 'vue';

const message = ref('');
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center">Email Change Confirmation</h1>
    <div class="text-center">{{ message }}</div>
  </DefaultLayout>
</template>
