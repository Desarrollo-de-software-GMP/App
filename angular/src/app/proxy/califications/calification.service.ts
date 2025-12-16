import type { CreateUpdateCalificationDTO } from './dtos/models';
import type { CalificationDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CalificationService {
  apiName = 'Default';
  

  create = (input: CreateUpdateCalificationDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificationDto>({
      method: 'POST',
      url: '/api/app/calification',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  obtenerPorUsuario = (usuarioId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificationDto[]>({
      method: 'POST',
      url: `/api/app/calification/obtener-por-usuario/${usuarioId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
