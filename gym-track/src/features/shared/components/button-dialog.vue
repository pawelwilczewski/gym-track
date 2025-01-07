<script setup lang="ts">
import { Button, ButtonVariants } from '@/features/shared/components/ui/button';
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogTitle,
} from '@/features/shared/components/ui/dialog';
import DialogDescription from '@/features/shared/components/ui/dialog/DialogDescription.vue';
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
      <slot name="dialog" :close-dialog="() => (open = false)" />
    </DialogContent>
  </Dialog>
</template>
