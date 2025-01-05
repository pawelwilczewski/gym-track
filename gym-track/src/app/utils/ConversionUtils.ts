import { UUID } from 'crypto';

export function toRecord<T, K extends string | number | UUID>(
  array: T[],
  keySelector: (item: T) => K
): Record<K, T> {
  return array.reduce<Record<K, T>>((acc, item) => {
    const key = keySelector(item);
    acc[key] = item;
    return acc;
  }, {});
}
