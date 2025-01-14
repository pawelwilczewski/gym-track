<script setup lang="ts">
import FooterContent from '@/features/shared/components/footer-content.vue';
import HeaderContent from '@/features/shared/components/header-content.vue';
</script>

<template>
  <div class="min-h-screen max-h-screen flex flex-col">
    <header class="container p-8">
      <HeaderContent />
    </header>
    <main class="container h-full">
      <slot />
    </main>
    <footer class="container mt-auto p-8">
      <FooterContent />
    </footer>
  </div>
</template>
