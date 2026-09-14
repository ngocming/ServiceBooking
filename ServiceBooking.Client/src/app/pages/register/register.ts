import { Component, inject } from '@angular/core';
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

  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        alert('Đăng ký tài khoản thành công! Vui lòng đăng nhập.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Error registering:', err);
        alert('Đăng ký thất bại. Vui lòng thử lại.');
      }
    });
  }
}
