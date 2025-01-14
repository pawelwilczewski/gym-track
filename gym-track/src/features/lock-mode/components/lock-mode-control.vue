<script setup lang="ts">
import { useLockMode } from '@/features/lock-mode/stores/use-lock-mode';
import { Lock, LockOpen } from 'lucide-vue-next';
import { Button } from '@/features/shared/components/ui/button';

const lockMode = useLockMode();
</script>

<template>
  <Button variant="ghost" @click="lockMode.toggle">
    <Lock v-if="lockMode.isLocked" class="w-6 h-6" />
    <LockOpen v-else class="w-6 h-6" />
  </Button>
</template>
