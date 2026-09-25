import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServerStatus {
  readonly isUnavailable = signal(false);

  markUnavailable(): void {
    this.isUnavailable.set(true);
  }

  markAvailable(): void {
    this.isUnavailable.set(false);
  }
}