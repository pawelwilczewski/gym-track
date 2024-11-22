import { z, ZodType } from 'zod';

export function zodEnumFlagsSchema<
  EnumObj extends Record<string, string | number>,
>(enumObj: EnumObj): ZodType {
  const enumValues = Object.values(enumObj).filter(
    (value): value is EnumObj[keyof EnumObj] & number =>
      typeof value === 'number'
  );
  const allValidBits = enumValues.reduce((acc, value) => acc | value, 0);
  return z
    .number()
    .int()
    .refine(
      (val): val is EnumObj[keyof EnumObj] & number =>
        (val & ~allValidBits) === 0,
      { message: 'Invalid flags value.' }
    );
}
