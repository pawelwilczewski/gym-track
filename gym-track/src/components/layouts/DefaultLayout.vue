<script setup lang="ts">
import FooterContent from '../FooterContent.vue';
import HeaderContent from '../HeaderContent.vue';
</script>

<template>
  <div class="min-h-screen grid grid-cols-1 grid-rows-auto-1fr-auto">
    <header class="maxed-content-width mx-auto p-8">
      <Suspense>
        <HeaderContent />
        <!--TODO Pawel: Skeleton for loading header (using <template #fallback>)-->
      </Suspense>
    </header>
    <main>
      <slot />
    </main>
    <footer class="maxed-content-width mx-auto p-8">
      <FooterContent />
    </footer>
  </div>
</template>
