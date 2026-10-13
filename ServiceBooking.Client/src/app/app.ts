import { Component, signal, inject, computed } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { ServerStatus } from './services/server-status/server-status';
import { Navbar } from './components/navbar/navbar';

@Component({
  imports: [RouterOutlet, Navbar],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('ServiceBooking.Client');
  protected readonly serverStatus = inject(ServerStatus);
  private router = inject(Router);

  // Ẩn navbar trên trang login và register
  private currentUrl = signal('');
  private readonly NO_NAVBAR_ROUTES = ['/login', '/register', '/forbidden'];

  showNavbar = computed(() => {
    const url = this.currentUrl();
    return !this.NO_NAVBAR_ROUTES.some(r => url.startsWith(r));
  });

  constructor() {
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: any) => {
        this.currentUrl.set(e.urlAfterRedirects ?? e.url);
      });
  }
}
