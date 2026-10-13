import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private router = inject(Router);

  ngOnInit(): void {
    const raw = localStorage.getItem('user');
    if (raw) {
      const user = JSON.parse(raw);
      if (user.role === 'Customer') {
        this.router.navigate(['/customer/services']);
      } else if (user.role === 'Provider') {
        this.router.navigate(['/provider/services']);
      } else {
        this.router.navigate(['/login']);
      }
    } else {
      this.router.navigate(['/login']);
    }
  }
}
