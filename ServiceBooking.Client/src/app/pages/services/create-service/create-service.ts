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
      validators: [
        Validators.required,
        Validators.minLength(3)
      ]
    }),

    description: new FormControl('', {
      nonNullable: true
    }),

    price: new FormControl(0, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(1)
      ]
    }),

    durationMinutes: new FormControl(30, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(1)
      ]
    })
  });

  createService() {
    if (this.serviceForm.invalid) {
      this.serviceForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const newService: CreateProviderServiceDTO = {
      name: this.serviceForm.value.name!,
      description: this.serviceForm.value.description!,
      price: this.serviceForm.value.price!,
      durationMinutes: this.serviceForm.value.durationMinutes!
    };

    this.providerServices.createProviderService(newService).subscribe({
      next: (response) => {
        this.loading = false;
        console.log('Service created successfully:', response);
        this.router.navigate(['/provider/services']);
      },
      error: (error) => {
        this.loading = false;
        console.error('Error creating service:', error);
        this.errorMessage = 'Lỗi khi tạo dịch vụ. Vui lòng thử lại.';
      }
    });
  }
}