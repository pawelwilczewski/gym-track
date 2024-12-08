<script setup lang="ts">
import FooterContent from '../app/FooterContent.vue';
import HeaderContent from '../app/HeaderContent.vue';
</script>

<template>
  <div class="min-h-screen grid grid-cols-1 grid-rows-auto-1fr-auto">
    <header class="container mx-auto p-8">
      <HeaderContent />
    </header>
    <main>
      <slot />
    </main>
    <footer class="container mx-auto p-8">
      <FooterContent />
    </footer>
  </div>
</template>
