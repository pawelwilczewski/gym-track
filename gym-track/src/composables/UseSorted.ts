import { computed, ComputedRef, Ref } from 'vue';

export function useSorted<T>(
  ref: Ref<T[]>,
  compareFn?: (a: T, b: T) => number
): {
  sorted: ComputedRef<T[]>;
} {
  return { sorted: computed(() => [...ref.value].sort(compareFn)) };
}
