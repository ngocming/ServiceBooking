import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Auth } from '../../services/auth/auth';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  private authService = inject(Auth);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  showPassword = false;
  isLoading = false;
  errorMessage = '';

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required),
    rememberMe: new FormControl(false),
  });

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify({
          id: response.id,
          username: response.username,
          email: response.email,
          role: response.role
        }));
        if (response.role === 'Customer') {
          this.router.navigate(['/bookings']);
        } else if (response.role === 'Provider') {
          this.router.navigate(['/provider/bookings']);
        } else if (response.role === 'Admin') {
          this.router.navigate(['/admin']);
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error logging in:', err);
        if (err.status === 0 || !err.status) {
          this.errorMessage = 'Máy chủ hiện không hoạt động (Server Offline). Vui lòng kiểm tra lại kết nối hoặc thử lại sau.';
        } else if (err.status === 401 || err.status === 400) {
          this.errorMessage = 'Email hoặc mật khẩu không chính xác.';
        } else {
          this.errorMessage = 'Đã có lỗi xảy ra. Vui lòng thử lại sau.';
        }
        this.cdr.markForCheck();
      }
    });
  }
}


