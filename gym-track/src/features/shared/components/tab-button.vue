<script setup lang="ts">
import router from '@/router';
import { Button } from '@/features/shared/components/ui/button';

const props = defineProps<{
  path: string;
  name: string;
}>();
</script>

<template>
  <Button
    :variant="
      router.currentRoute.value.path === props.path ? 'secondary' : 'ghost'
    "
    as-child
  >
    <RouterLink :to="props.path">{{ props.name }}</RouterLink>
  </Button>
</template>
