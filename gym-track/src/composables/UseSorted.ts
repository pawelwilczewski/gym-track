import { UUID } from 'crypto';
import { computed, ComputedRef, Ref } from 'vue';

type RecordKey = string | symbol | number | UUID;

export function useSorted<T>(
  record: Ref<Record<RecordKey, T>>,
  compareFn: (a: T, b: T) => number
): ComputedRef<T[]> {
  return computed(() => Object.values(record.value).sort(compareFn));
}
