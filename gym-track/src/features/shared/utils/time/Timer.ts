import { getCurrentTimeSeconds } from '@/features/shared/utils/time/time-utils';

export default class Timer {
  private totalDurationSeconds: number;
  private tickIntervalSeconds: number;
  private onTick?: (timeLeft: number) => void;
  private onComplete?: () => void;
  private timeStartedSeconds?: number;
  private intervalId?: NodeJS.Timeout;

  public constructor(
    totalDurationSeconds: number,
    tickIntervalSeconds: number,
    onTick?: (timeLeft: number) => void,
    onComplete?: () => void
  ) {
    if (totalDurationSeconds < 0) {
      throw new Error('Invalid total time');
    }
    if (tickIntervalSeconds < 0) {
      throw new Error('Invalid interval');
    }

    this.totalDurationSeconds = totalDurationSeconds;
    this.tickIntervalSeconds = tickIntervalSeconds;
    this.onTick = onTick;
    this.onComplete = onComplete;
  }

  public start(): void {
    if (this.isRunning()) {
      return;
    }

    this.timeStartedSeconds = getCurrentTimeSeconds();

    this.intervalId = setInterval(
      this.handleTick,
      this.tickIntervalSeconds * 1000
    );
  }

  private handleTick = (): void => {
    const currentTimeLeft = this.timeLeft();

    if (this.onTick) {
      this.onTick(currentTimeLeft);
    }

    if (currentTimeLeft >= this.tickIntervalSeconds) {
      return;
    }

    clearInterval(this.intervalId);

    this.intervalId = setInterval(() => {
      if (this.onComplete) {
        clearInterval(this.intervalId);
        this.onComplete();
      }
    }, currentTimeLeft * 1000);
  };

  public timeLeft(): number {
    const timeElapsed = getCurrentTimeSeconds() - this.timeStartedSeconds!;
    return this.totalDurationSeconds - timeElapsed;
  }

  public isRunning(): boolean {
    return this.timeLeft() > 0;
  }
}
