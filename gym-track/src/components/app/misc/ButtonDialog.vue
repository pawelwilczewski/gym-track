<script setup lang="ts">
import { Button, ButtonVariants } from '@/components/ui/button';
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogTitle,
} from '@/components/ui/dialog';
import DialogDescription from '@/components/ui/dialog/DialogDescription.vue';
const open = defineModel<boolean>();
const { variant = 'outline' } = defineProps<{
  dialogTitle: string;
  variant?: ButtonVariants['variant'];
}>();
</script>

<template>
  <Dialog v-model:open="open">
    <DialogTrigger>
      <Button :variant="variant"><slot name="button" /></Button>
    </DialogTrigger>
    <DialogContent>
      <DialogDescription></DialogDescription>
      <DialogTitle>{{ dialogTitle }}</DialogTitle>
      <slot name="dialog" />
    </DialogContent>
  </Dialog>
</template>
