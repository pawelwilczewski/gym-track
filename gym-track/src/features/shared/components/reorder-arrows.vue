<script setup lang="ts" generic="TContext">
import Button from '@/features/shared/components/ui/button/Button.vue';
import { ArrowDown, ArrowUp } from 'lucide-vue-next';

const { context } = defineProps<{
  context?: TContext;
}>();

const emit = defineEmits<{
  up: [TContext?];
  down: [TContext?];
}>();
</script>

<template>
  <Button variant="ghost" @click="emit('up', context)">
    <ArrowUp class="w-4 h-4" />
  </Button>

  <Button variant="ghost" @click="emit('down', context)">
    <ArrowDown class="w-4 h-4" />
  </Button>
</template>
