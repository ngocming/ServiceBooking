import { Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Provider, ProviderProfile, UpdateProviderDto } from '../../services/provider/provider';

@Component({
  selector: 'app-provider-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './provider-profile.html',
  styleUrl: './provider-profile.css'
})
export class ProviderProfilePage implements OnInit {
  private providerService = inject(Provider);

  profile = signal<ProviderProfile | null>(null);
  loading = signal(true);
  saving = signal(false);
  successMessage = signal('');
  errorMessage = signal('');

  profileForm = new FormGroup({
    displayName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    description: new FormControl(''),
    phone: new FormControl('', [Validators.required, Validators.pattern(/^[0-9+() -]{8,15}$/)]),
    address: new FormControl('', [Validators.required]),
    isAvailable: new FormControl(true)
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.providerService.getMyProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.profileForm.patchValue({
          displayName: data.displayName || '',
          description: data.description || '',
          phone: data.phone || '',
          address: data.address || '',
          isAvailable: data.isAvailable
        });
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error fetching provider profile:', err);
        this.errorMessage.set('Không thể tải thông tin hồ sơ. Vui lòng thử lại sau.');
        this.loading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.successMessage.set('');
    this.errorMessage.set('');

    const formVal = this.profileForm.value;
    const updateDto: UpdateProviderDto = {
      displayName: formVal.displayName || '',
      description: formVal.description || '',
      phone: formVal.phone || '',
      address: formVal.address || '',
      isAvailable: formVal.isAvailable ?? true
    };

    this.providerService.updateMyProfile(updateDto).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.saving.set(false);
        this.successMessage.set('Cập nhật hồ sơ thành công!');
        setTimeout(() => this.successMessage.set(''), 4000);
      },
      error: (err) => {
        console.error('Error updating profile:', err);
        this.saving.set(false);
        this.errorMessage.set('Cập nhật hồ sơ thất bại. Vui lòng kiểm tra lại.');
      }
    });
  }
}
