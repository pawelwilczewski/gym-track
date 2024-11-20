export function roundToMultiple(x: number, multiple: number): number {
  const whole = Math.floor(x / multiple);
  const mod = x % multiple;
  return mod >= multiple * 0.5 ? whole * multiple + multiple : whole * multiple;
}
