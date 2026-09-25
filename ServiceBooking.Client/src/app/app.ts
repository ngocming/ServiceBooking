import { Component, signal,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ServerStatus } from './services/server-status/server-status';


@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('ServiceBooking.Client');
  protected readonly serverStatus = inject(ServerStatus);
}
