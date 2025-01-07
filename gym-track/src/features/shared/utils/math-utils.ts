export function roundToMultiple(x: number, multiple: number): number {
  const whole = Math.floor(x / multiple);
  // eslint-disable-next-line unicorn/prevent-abbreviations
  const mod = x % multiple;
  return mod >= multiple * 0.5 ? whole * multiple + multiple : whole * multiple;
}
