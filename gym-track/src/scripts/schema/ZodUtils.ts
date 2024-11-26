import { z, ZodType } from 'zod';

export type EnumFlags = Record<string, string | number>;
export type EnumFlagsNumber = EnumFlags[keyof EnumFlags] & number;

export function zodEnumFlagsAsNumber(enumObj: EnumFlags): ZodType {
  const allValidBits = getAllValidEnumBits(enumObj);
  return z
    .number()
    .int()
    .refine(
      (value): value is EnumFlagsNumber => (value & ~allValidBits) === 0,
      { message: 'Invalid flags value.' }
    );
}

export function getAllValidEnumBits(enumObj: EnumFlags): number {
  return Object.values(enumObj)
    .filter((value): value is EnumFlagsNumber => typeof value === 'number')
    .reduce((acc, value) => acc | value, 0);
}

function calculateBitSumFromStringArray(
  stringArray: string[],
  allValidBits: number
): number {
  const bitSum = stringArray
    .map(str => {
      return parseInt(str, 10);
    })
    .reduce((sum: number, num: number) => {
      if (isNaN(num) || (num & ~allValidBits) !== 0) {
        throw new Error('Invalid flags in string array.');
      }
      return sum + num;
    }, 0);
  return bitSum;
}

export function zodEnumFlagsAsStringArray(enumObj: EnumFlags): ZodType {
  const allValidBits = getAllValidEnumBits(enumObj);
  return z
    .array(z.string())
    .refine(
      value =>
        value.every(str => {
          const num = parseInt(str, 10);
          return !isNaN(num) && (num & ~allValidBits) === 0;
        }),
      { message: 'Invalid flags in string array.' }
    )
    .transform(value => {
      return calculateBitSumFromStringArray(value, allValidBits);
    })
    .refine(value => value > 0, { message: 'Required' });
}
