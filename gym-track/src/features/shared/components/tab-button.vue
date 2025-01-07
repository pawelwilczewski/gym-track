<script setup lang="ts">
import router from '@/router';
import { Button } from '@/features/shared/components/ui/button';

const { path, name } = defineProps<{
  path: string;
  name: string;
}>();
</script>

<template>
  <Button
    :variant="router.currentRoute.value.path === path ? 'secondary' : 'ghost'"
    as-child
  >
    <RouterLink :to="path">{{ name }}</RouterLink>
  </Button>
</template>
