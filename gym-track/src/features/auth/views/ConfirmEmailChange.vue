<script setup lang="ts">
import DefaultLayout from '@/features/shared/layouts/DefaultLayout.vue';
import { ref } from 'vue';

const message = ref('');
</script>

<template>
  <DefaultLayout>
    <h1 class="text-center">Email Change Confirmation</h1>
    <div class="text-center">{{ message }}</div>
  </DefaultLayout>
</template>
