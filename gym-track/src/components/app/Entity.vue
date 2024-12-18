<script setup lang="ts">
import { Button } from '@/components/ui/button';
import { Pencil, Trash2 } from 'lucide-vue-next';
import ButtonDialog from '@/components/app/misc/ButtonDialog.vue';
import { type IsComponentType } from '@/app/utils/ComponentTypes';

const { is = 'div' } = withDefaults(
  defineProps<{
    is?: IsComponentType;
    editable?: boolean;
    deletable?: boolean;
  }>(),
  {
    editable: true,
    deletable: true,
  }
);

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
      <Button
        v-if="deletable === undefined || deletable"
        variant="ghost"
        size="sm"
        @click="emit('deleted')"
      >
        <Trash2 class="w-4 h-4" />
      </Button>
    </div>
  </component>
</template>
