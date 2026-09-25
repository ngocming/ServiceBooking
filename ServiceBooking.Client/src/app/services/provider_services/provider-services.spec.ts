import { TestBed } from '@angular/core/testing';
import { ProviderServices } from './provider-services';

describe('ProviderServices', () => {
  let service: ProviderServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProviderServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
