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

    const user = localStorage.getItem('user');

    if (!user) {
      this.errorMessage = 'Không tìm thấy thông tin người dùng.';
      return;
    }

    const userData = JSON.parse(user);

    const serviceData: CreateProviderServiceDTO = {
      name: this.serviceForm.controls.name.value,
      description: this.serviceForm.controls.description.value,
      price: this.serviceForm.controls.price.value,
      durationMinutes:
        this.serviceForm.controls.durationMinutes.value,
      providerId: userData.providerId
    };

    this.loading = true;
    this.errorMessage = '';

    this.providerServices
      .createProviderService(serviceData)
      .subscribe({
        next: () => {
          this.loading = false;

          this.router.navigate(['/services']);
        },

        error: (error) => {
          console.error(error);

          this.loading = false;
          this.errorMessage =
            'Không thể tạo dịch vụ.';
        }
      });
  }
}