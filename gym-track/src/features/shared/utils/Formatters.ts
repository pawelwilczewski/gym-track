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

export function formatDateTime(dateTime: Date): string {
  return dateTime.toLocaleString(navigator.language, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function formatDuration(time: string): string {
  const timeSplit = time.split(':').map(Number);
  const parts = [];

  if (timeSplit[0] > 0) {
    parts.push(`${timeSplit[0]} hours`);
  }
  if (timeSplit[1] > 0) {
    parts.push(`${timeSplit[1]} minutes`);
  }
  if (timeSplit[2] > 0) {
    parts.push(`${timeSplit[2]} seconds`);
  }

  if (parts.length === 0) {
    return '0 seconds';
  }
  if (parts.length === 1) {
    return parts[0];
  }

  const lastPart = parts.pop();
  return `${parts.join(', ')} and ${lastPart}`;
}
