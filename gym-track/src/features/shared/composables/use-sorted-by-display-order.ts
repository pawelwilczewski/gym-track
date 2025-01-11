import { useSorted } from '@vueuse/core';
import { Ref } from 'vue';

type DisplayOrdered = { displayOrder: number };

export function useSortedByDisplayOrder<T extends DisplayOrdered>(
  array: Ref<T[]>
): Ref<T[]> {
  return useSorted(array, (a, b) => a.displayOrder - b.displayOrder);
}
