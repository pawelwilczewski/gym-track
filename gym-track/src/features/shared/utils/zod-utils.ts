import { z, ZodType } from 'zod';

type EnumFlags = Record<string, string | number>;
type EnumFlagsNumber = EnumFlags[keyof EnumFlags] & number;

/**
 * Accepts an int enum flag value and checks if it's
 * valid for the given enum type.
 */
export function zodEnumFlagsAsNumber(
  enumType: EnumFlags,
  isRequired: boolean = true
): ZodType {
  const allValidBits = getAllValidEnumBits(enumType);
  return z
    .number()
    .int()
    .refine(
      (value): value is EnumFlagsNumber => (value & ~allValidBits) === 0,
      { message: 'Invalid flags value.' }
    )
    .refine(value => isRequired && value > 0, { message: 'Required' });
}

/**
 * Accepts enum flag string array input, i.e. `['1', '2', '8']`, checks if they
 * are valid enum flags values for the given enum type, and returns the sum of
 * all given flags, i.e. `['1', '2', '8'] -> 11`, so it can be worked with like
 * an enum flag value.
 *
 * This can be used with toggle groups.
 */
export function zodEnumFlagsAsStringArray(
  enumType: EnumFlags,
  isRequired: boolean = true
): ZodType {
  const allValidBits = getAllValidEnumBits(enumType);
  return z
    .array(z.string())
    .refine(
      value =>
        value.every(string_ => {
          const number_ = Number.parseInt(string_, 10);
          return !Number.isNaN(number_) && (number_ & ~allValidBits) === 0;
        }),
      { message: 'Invalid flags in string array.' }
    )
    .transform(value => {
      return calculateBitSumFromStringArray(value, allValidBits);
    })
    .refine(value => isRequired && value > 0, { message: 'Required' });
}

export function getAllValidEnumBits(enumType: EnumFlags): number {
  return Object.values(enumType)
    .filter((value): value is EnumFlagsNumber => typeof value === 'number')
    .reduce((accumulator, value) => accumulator | value, 0);
}

function calculateBitSumFromStringArray(
  stringArray: string[],
  allValidBits: number
): number {
  const bitSum = stringArray
    .map(string_ => {
      return Number.parseInt(string_, 10);
    })
    // eslint-disable-next-line unicorn/no-array-reduce
    .reduce((sum: number, number_: number) => {
      if (Number.isNaN(number_) || (number_ & ~allValidBits) !== 0) {
        throw new Error('Invalid flags in string array.');
      }
      return sum + number_;
    }, 0);
  return bitSum;
}

function enumFlagsValueToBitValuesArray(value: number): number[] {
  const result: number[] = [];
  let currentBit = 1;

  while (value > 0) {
    if (value & 1) {
      result.push(currentBit);
    }
    value >>= 1;
    currentBit <<= 1;
  }

  return result;
}

export function enumFlagsValueToStringArray(value: number): string[] {
  return enumFlagsValueToBitValuesArray(value).map(value_ => value_.toString());
}
