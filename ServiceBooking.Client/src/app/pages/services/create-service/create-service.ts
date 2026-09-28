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

    console.log('Form valid:', this.serviceForm.value);
  }
}