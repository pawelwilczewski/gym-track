import {
  DistanceUnit,
  WeightUnit,
} from '@/features/exercise-info/types/exercise-info-types';

export function formatWeight(weight: number, unit: WeightUnit): string {
  let unitText;
  switch (unit) {
    case WeightUnit.Kilogram: {
      unitText = 'kg';
      break;
    }
    case WeightUnit.Pound: {
      unitText = 'lbs';
      break;
    }
  }
  return `${weight} ${unitText}`;
}

export function formatDistance(distance: number, unit: DistanceUnit): string {
  let unitText;
  switch (unit) {
    case DistanceUnit.Metre: {
      unitText = 'm';
      break;
    }
    case DistanceUnit.Yard: {
      unitText = 'yd';
      break;
    }
  }
  return `${distance} ${unitText}`;
}
