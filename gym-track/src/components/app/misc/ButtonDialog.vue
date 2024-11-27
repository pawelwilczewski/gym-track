<script setup lang="ts">
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogTitle,
} from '@/components/ui/dialog';
const open = defineModel<boolean>();
defineProps<{
  buttonText: string;
  dialogTitle: string;
}>();
</script>

<template>
  <Dialog v-model:open="open">
    <DialogTrigger>
      <Button variant="outline">{{ buttonText }}</Button>
    </DialogTrigger>
    <DialogContent>
      <DialogTitle>{{ dialogTitle }}</DialogTitle>
      <slot />
    </DialogContent>
  </Dialog>
</template>
