import { useSorted } from '@vueuse/core';
import { computed, ComputedRef, Ref } from 'vue';

type DisplayOrdered = { displayOrder: number };

export function useSortedByDisplayOrder<T extends DisplayOrdered>(
  array: Ref<T[]>
): Ref<T[]> {
  return useSorted(array, (a, b) => a.displayOrder - b.displayOrder);
}

// TODO Pawel: possibly extract useSortedRecord
export function useSortedByDisplayOrderRecord<
  TKey extends string,
  TValue extends DisplayOrdered,
>(record: Ref<Record<TKey, TValue>>): ComputedRef<Record<TKey, TValue>> {
  return computed(() => {
    const sorted = Object.entries(record.value).sort(
      ([, a], [, b]) => (a as TValue).displayOrder - (b as TValue).displayOrder
    );
    return Object.fromEntries(sorted) as Record<TKey, TValue>;
  });
}
