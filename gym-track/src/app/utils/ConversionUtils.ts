import { UUID } from 'crypto';

export function toRecord<TValue, TKey extends string | number | UUID>(
  array: TValue[],
  keySelector: (item: TValue) => TKey
): Record<TKey, TValue> {
  return array.reduce<Record<TKey, TValue>>(
    (acc, item) => {
      const key = keySelector(item);
      acc[key] = item;
      return acc;
    },
    {} as Record<TKey, TValue>
  );
}
