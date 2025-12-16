
export interface CityDto {
  name?: string;
  country?: string;
  population: number;
  latitude: number;
  longitude: number;
  photoUrl?: string;
}

export interface CitySearchRequestDTO {
  partialName?: string;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}
