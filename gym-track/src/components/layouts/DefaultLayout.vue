<script setup lang="ts">
import FooterContent from '../FooterContent.vue';
import HeaderContent from '../HeaderContent.vue';
</script>

<template>
  <div class="min-h-screen grid grid-cols-1 grid-rows-auto-1fr-auto">
    <header class="maxed-content-width mx-auto">
      <HeaderContent />
    </header>
    <main class="mt-16">
      <slot />
    </main>
    <footer class="maxed-content-width mx-auto">
      <FooterContent />
    </footer>
  </div>
</template>
