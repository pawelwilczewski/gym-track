import { UUID } from 'node:crypto';

export function toRecord<TValue, TKey extends string | number | UUID>(
  array: TValue[],
  keySelector: (item: TValue) => TKey
): Record<TKey, TValue> {
  // eslint-disable-next-line unicorn/no-array-reduce
  return array.reduce<Record<TKey, TValue>>(
    (accumulator, item) => {
      const key = keySelector(item);
      accumulator[key] = item;
      return accumulator;
    },
    {} as Record<TKey, TValue>
  );
}
