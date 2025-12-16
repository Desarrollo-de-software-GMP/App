import type { AuditedEntityDto } from '@abp/ng.core';

export interface CoordinatesDTO {
  latitude: number;
  longitude: number;
}

export interface CreateUpdateDestinationDTO {
  name: string;
  country: string;
  poblation: number;
  photoUrl: string;
  lastUpdate: string;
  coordinates: CoordinatesDTO;
}

export interface DestinationDTO extends AuditedEntityDto<string> {
  name?: string;
  country: string;
  poblation: number;
  photoUrl?: string;
  lastUpdate?: string;
  coordinates: CoordinatesDTO;
}
