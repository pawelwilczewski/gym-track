<script setup lang="ts">
import { Pencil, Trash2 } from 'lucide-vue-next';
import ButtonDialog from '@/features/shared/components/button-dialog.vue';
import { type IsComponentType } from '@/features/shared/utils/component-types';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/features/shared/components/ui/alert-dialog';
import { Button, buttonVariants } from '@/features/shared/components/ui/button';

const {
  is = 'div',
  editable = true,
  deletable = true,
} = defineProps<{
  is?: IsComponentType;
  editable?: boolean;
  deletable?: boolean;
}>();

const emit = defineEmits<{
  deleted: [];
}>();
</script>

<template>
  <component :is="is" class="relative">
    <slot />
    <div class="absolute top-0 right-0 flex">
      <ButtonDialog
        v-if="editable === undefined || editable"
        dialog-title="Edit"
        variant="ghost"
      >
        <template #dialog="{ closeDialog }">
          <slot name="edit" :close-dialog="closeDialog">
            Your edit form goes here.
          </slot>
        </template>
        <template #button><Pencil class="w-4 h-4" /></template>
      </ButtonDialog>

      <AlertDialog v-if="deletable === undefined || deletable">
        <AlertDialogTrigger as-child>
          <Button variant="ghost">
            <Trash2 class="w-4 h-4" />
          </Button>
        </AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the
              asset.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction as-child>
              <Button
                :class="buttonVariants({ variant: 'destructive' })"
                @click="emit('deleted')"
              >
                Delete
              </Button>
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  </component>
</template>
