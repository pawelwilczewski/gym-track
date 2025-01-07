<script setup lang="ts">
import { cn } from '@/features/shared/utils/cn-utils';
import {
  ToastRoot,
  type ToastRootEmits,
  useForwardPropsEmits,
} from 'radix-vue';
import { computed } from 'vue';
import { type ToastProps as ToastProperties, toastVariants } from '.';

const props = defineProps<ToastProperties>();

const emits = defineEmits<ToastRootEmits>();

const delegatedProperties = computed(() => {
  const { class: _, ...delegated } = props;

  return delegated;
});

const forwarded = useForwardPropsEmits(delegatedProperties, emits);
</script>

<template>
  <ToastRoot
    v-bind="forwarded"
    :class="cn(toastVariants({ variant }), props.class)"
    @update:open="onOpenChange"
  >
    <slot />
  </ToastRoot>
</template>
