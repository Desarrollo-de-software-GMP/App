import type { CreateUpdateCalificationDTO } from './dtos/models';
import type { CalificationDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CreateUpdateCalificacionService {
  apiName = 'Default';
  

  createCalification = (input: CreateUpdateCalificationDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificationDto>({
      method: 'POST',
      url: '/api/app/create-update-calificacion/calification',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
