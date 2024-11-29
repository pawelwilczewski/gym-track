<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { Pencil, Trash2 } from 'lucide-vue-next';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';

const { is = 'div' } = defineProps<{
  is?: 'div' | 'li';
}>();

const emit = defineEmits<{
  delete: [];
}>();
</script>

<template>
  <component :is="is" class="relative">
    <slot />
    <div class="absolute top-0 right-0 flex">
      <ButtonDialog dialogTitle="Edit" variant="ghost">
        <template #dialog>
          <slot name="edit">Your edit form goes here.</slot>
        </template>
        <template #button><Pencil class="w-4 h-4" /></template>
      </ButtonDialog>
      <Button @click="emit('delete')" variant="ghost" size="sm"
        ><Trash2 class="w-4 h-4" />
      </Button>
    </div>
  </component>
</template>
