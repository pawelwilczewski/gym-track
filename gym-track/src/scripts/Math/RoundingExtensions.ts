export {};

declare global {
  interface Number {
    roundToMultiple(multiple: number): number;
  }
}

Object.defineProperty(Number.prototype, 'roundToMultiple', {
  value: function roundToMultiple(multiple: number): number {
    const whole = Math.floor(this / multiple);
    const mod = this % multiple;
    return mod >= multiple * 0.5
      ? whole * multiple + multiple
      : whole * multiple;
  },
});
