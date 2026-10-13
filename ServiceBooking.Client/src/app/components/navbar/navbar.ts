import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  imports: [RouterLink],
  selector: 'app-navbar',
  styleUrl: './navbar.css',
  templateUrl: './navbar.html',
})
export class Navbar implements OnInit {
  private router = inject(Router);

  userRole = signal('');
  username = signal('');
  isMenuOpen = signal(false);

  ngOnInit(): void {
    this.loadUser();
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => this.loadUser());
  }

  loadUser(): void {
    const raw = localStorage.getItem('user');
    if (raw) {
      const user = JSON.parse(raw);
      this.userRole.set(user.role ?? '');
      this.username.set(user.username ?? '');
    } else {
      this.userRole.set('');
      this.username.set('');
    }
  }

  toggleMenu(): void {
    this.isMenuOpen.update(v => !v);
  }

  closeMenu(): void {
    this.isMenuOpen.set(false);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.userRole.set('');
    this.username.set('');
    this.router.navigate(['/login']);
  }
}
