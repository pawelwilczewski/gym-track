<script setup lang="ts">
import DefaultLayout from '@/components/layouts/DefaultLayout.vue';
import { ref } from 'vue';

const message = ref('');
</script>

<template>
  <DefaultLayout>
    <h1 class="text-xl font-bold text-center">Email Change Confirmation</h1>
    <div class="text-center">{{ message }}</div>
  </DefaultLayout>
</template>
