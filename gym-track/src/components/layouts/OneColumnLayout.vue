<script setup lang="ts">
import DefaultLayout from './DefaultLayout.vue';
</script>

<template>
  <DefaultLayout>
    <div class="maxed-content-width mx-auto p-8">
      <slot />
    </div>
  </DefaultLayout>
</template>
