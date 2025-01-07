<script setup lang="ts">
import FooterContent from '@/features/shared/components/footer-content.vue';
import HeaderContent from '@/features/shared/components/header-content.vue';
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
