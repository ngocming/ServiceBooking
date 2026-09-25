import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Auth } from '../../services/auth/auth';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-register',
  styleUrl: './register.css',
  templateUrl: './register.html',
})
export class Register {
  private authService = inject(Auth);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  showPassword = false;
  isLoading = false;
  errorMessage = '';

  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    role: new FormControl('Customer', [Validators.required]),
  });

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  selectRole(role: string): void {
    this.registerForm.patchValue({ role });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/login']);
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error registering:', err);
        if (err.status === 0 || !err.status) {
          this.errorMessage = 'Máy chủ hiện không hoạt động (Server Offline). Vui lòng kiểm tra lại kết nối hoặc thử lại sau.';
        } else if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else {
          this.errorMessage = 'Đăng ký thất bại. Email hoặc thông tin có thể đã tồn tại.';
        }
        this.cdr.markForCheck();
      }
    });
  }
}


