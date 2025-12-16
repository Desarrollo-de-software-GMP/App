import type { AuditedEntityDto } from '@abp/ng.core';

export interface CalificationDto extends AuditedEntityDto<string> {
  puntuation: number;
  comment?: string;
  userId?: string;
  destinoId?: string;
}
