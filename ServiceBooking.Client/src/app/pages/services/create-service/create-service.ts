import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import {
  ProviderServices,
  CreateProviderServiceDTO
} from '../../../services/provider_services/provider-services';
import { CategoryService, CategoryItem } from '../../../services/category/category';

@Component({
  selector: 'app-create-service',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-service.html',
  styleUrl: './create-service.css'
})
export class CreateService implements OnInit {

  private providerServices = inject(ProviderServices);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  categories = signal<CategoryItem[]>([]);
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

    categoryId: new FormControl<number | null>(null),

    description: new FormControl('', {
      nonNullable: true
    }),

    imageUrl: new FormControl('', {
      nonNullable: true
    }),

    price: new FormControl(0, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(1)
      ]
    }),

    durationMinutes: new FormControl(60, {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.min(1)
      ]
    })
  });

  ngOnInit(): void {
    this.categoryService.getCategories(true).subscribe({
      next: (cats) => {
        this.categories.set(cats);
        if (cats.length > 0) {
          this.serviceForm.patchValue({ categoryId: cats[0].id });
        }
      },
      error: (err) => {
        console.warn('Could not load categories:', err);
      }
    });
  }

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
      durationMinutes: this.serviceForm.value.durationMinutes!,
      categoryId: this.serviceForm.value.categoryId || undefined,
      imageUrl: this.serviceForm.value.imageUrl || undefined
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
        this.errorMessage = error.error?.message || error.error || 'Có lỗi xảy ra khi tạo dịch vụ. Vui lòng thử lại.';
      }
    });
  }
}
