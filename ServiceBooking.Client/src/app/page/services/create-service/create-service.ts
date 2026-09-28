import { Component, inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import {
  ProviderServices,
  CreateProviderServiceDTO
} from '../../../services/provider_services/provider-services';

@Component({
  selector: 'app-create-service',
  imports: [ReactiveFormsModule],
  templateUrl: './create-service.html',
  styleUrl: './create-service.css'
})
export class CreateService {
  private providerServices = inject(ProviderServices);
  private router = inject(Router);

  loading = false;
  errorMessage = '';

  serviceForm = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(3)]
    }),
    description: new FormControl('', {
      nonNullable: true
    }),
    price: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1)]
    }),
    durationMinutes: new FormControl(30, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1)]
    })
  });

  createService() {
    if (this.serviceForm.invalid) {
      this.serviceForm.markAllAsTouched();
      return;
    }

    const serviceData: CreateProviderServiceDTO = {
      name: this.serviceForm.controls.name.value,
      description: this.serviceForm.controls.description.value,
      price: this.serviceForm.controls.price.value,
      durationMinutes: this.serviceForm.controls.durationMinutes.value
    };

    this.loading = true;
    this.errorMessage = '';

    this.providerServices.createProviderService(serviceData).subscribe({
      next: (createdService) => {
        console.log('Created service:', createdService);
        this.loading = false;
        this.router.navigate(['/services']);
      },
      error: (error) => {
        console.error('Create service error:', error);
        this.loading = false;

        if (error.status === 400) {
          this.errorMessage = error.error || 'Dữ liệu dịch vụ không hợp lệ.';
        } else if (error.status === 401) {
          this.errorMessage = 'Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.';
        } else if (error.status === 403) {
          this.errorMessage = 'Tài khoản hiện tại không có quyền tạo dịch vụ.';
        } else {
          this.errorMessage = 'Không thể tạo dịch vụ. Vui lòng thử lại.';
        }
      }
    });
  }
}
