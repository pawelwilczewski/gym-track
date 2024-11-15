export function roundToMultiple(toRound: number, multiple: number): number {
  const whole = Math.floor(toRound / multiple);
  const mod = toRound % multiple;
  return mod >= multiple * 0.5 ? whole * multiple + multiple : whole * multiple;
}
