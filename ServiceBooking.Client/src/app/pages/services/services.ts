import { Component } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-services',
  styleUrl: './services.css',
  templateUrl: './services.html',
})
export class Services {
  services = [
    {
      id: 1,
      name: 'Sửa xe máy',
      price: 150000,
      provider: 'Nguyễn Văn A'
    },
    {
      id: 2,
      name: 'Bảo dưỡng xe máy',
      price: 300000,
      provider: 'Trần Văn B'
    },
    {
      id: 3,
      name: 'Thay dầu nhớt',
      price: 100000,
      provider: 'Lê Văn C'
    }
  ];
}
