import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService } from '@abp/ng.theme.shared';
import { finalize } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

import { DestinationService } from '../../proxy/application/destinations/destination.service';
import { CityDto, CitySearchRequestDTO } from '../../proxy/destinations/models';
import { CreateUpdateDestinationDTO } from '../../proxy/models';

interface SearchParams {
  skipCount: number;
  maxResultCount: number;
  query: string;
  country: string;
}

@Component({
  selector: 'app-destinations-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CoreModule,
    NgbPaginationModule
  ],
  templateUrl: './destinations-list.component.html',
  styleUrls: ['./destinations-list.component.scss'],
})
export class DestinationsListComponent implements OnInit {
  private readonly destinationService = inject(DestinationService);
  private readonly toaster = inject(ToasterService);

  destinations: CityDto[] = [];
  localDestinations: CityDto[] = []; // Lista para control de duplicados
  
  loading = false;
  totalCount = 0;
  currentPage = 1;
  isLocalSource = true;
  
  savedItems = new Set<string>();
  readonly defaultImage = 'assets/images/logo/icon.svg';

  searchParams: SearchParams = {
    skipCount: 0,
    maxResultCount: 10,
    query: '',
    country: '',
  };

  ngOnInit(): void {
    this.loadLocalDestinations();
    this.loadDestinations();
  }

  // Carga destinos locales para validar duplicados por coordenadas
  private loadLocalDestinations(): void {
    this.destinationService.getList({ maxResultCount: 1000 } as any).subscribe({
      next: (result) => {
        this.localDestinations = (result.items || []).map(this.normalizeCityData);
      }
    });
  }

  // Normalización de datos (incluye Mapa en Español)
  private normalizeCityData = (item: any): CityDto => {
    const rawLat = item.latitude ?? item.Latitude ?? item.coordinates?.latitude ?? item.Coordinates?.Latitude;
    const rawLng = item.longitude ?? item.Longitude ?? item.coordinates?.longitude ?? item.Coordinates?.Longitude;
    
    const lat = typeof rawLat !== 'undefined' ? parseFloat(rawLat) : 0;
    const lng = typeof rawLng !== 'undefined' ? parseFloat(rawLng) : 0;

    const rawPop = item.poblation ?? item.Poblation ?? item.population ?? item.Population;
    const population = typeof rawPop !== 'undefined' ? parseInt(rawPop, 10) : 0;

    let imageUrl = item.photoUrl || item.PhotoUrl || item.imageUrl || item.ImageUrl;
    
    // Generar mapa estático en ESPAÑOL si no hay imagen
    if (!imageUrl && lat !== 0 && lng !== 0) {
        imageUrl = `https://static-maps.yandex.ru/1.x/?ll=${lng},${lat}&z=11&l=map&lang=es_ES&size=450,300&pt=${lng},${lat},pm2rdm`;
    }

    return {
      ...item,
      name: item.name || item.Name || item.city || item.City,
      country: item.country || item.Country,
      population: population,
      latitude: lat,
      longitude: lng,
      imageUrl: imageUrl
    } as CityDto;
  }

  private loadDestinations(): void {
    this.loading = true;
    let requestObservable;

    // Lógica de selección de servicio (API Externa vs Local)
    if (this.searchParams.query && this.searchParams.query.trim().length > 0) {
      console.log('🌐 Buscando ciudades externas...');
      this.isLocalSource = false;
      const searchRequest: CitySearchRequestDTO = { partialName: this.searchParams.query };
      requestObservable = this.destinationService.searchCities(searchRequest);
    } else {
      console.log('🏠 Cargando destinos locales...');
      this.isLocalSource = true;
      requestObservable = this.destinationService.getList(this.searchParams as any);
    }

    requestObservable
      .pipe(finalize(() => { this.loading = false; }))
      .subscribe({
        next: (result: any) => {
          const rawItems = result.items || result.cities || (Array.isArray(result) ? result : []);
          
          // 1. Normalizar datos
          let processedItems = rawItems.map(this.normalizeCityData);

          // 2. FILTRADO POR PAÍS (Lógica solicitada)
          // Si hay texto en el campo 'country', filtramos los resultados que empiecen con ese texto
          if (this.searchParams.country && this.searchParams.country.trim().length > 0) {
            const countryFilter = this.searchParams.country.toLowerCase().trim();
            processedItems = processedItems.filter(item => 
              item.country && item.country.toLowerCase().startsWith(countryFilter)
            );
          }

          // 3. Ordenar por población
          processedItems.sort((a, b) => (b.population || 0) - (a.population || 0));

          // 4. Asignar a la vista
          this.destinations = processedItems;
          
          // Ajustamos el totalCount al número de items filtrados para que la paginación sea coherente
          // con lo que se ve en pantalla en este contexto de búsqueda mixta.
          this.totalCount = processedItems.length;
        },
        error: (error) => {
          console.error('❌ Error al cargar:', error);
          this.destinations = [];
          this.totalCount = 0;
        },
      });
  }

  // Chequeo de duplicados (Nombre en sesión O Coordenadas en BD)
  isDuplicate(destination: CityDto): boolean {
    if (this.isLocalSource) return true;
    if (destination.name && this.savedItems.has(destination.name)) return true;

    const epsilon = 0.001; 
    return this.localDestinations.some(local => {
      const latDiff = Math.abs((local.latitude || 0) - (destination.latitude || 0));
      const lngDiff = Math.abs((local.longitude || 0) - (destination.longitude || 0));
      return latDiff < epsilon && lngDiff < epsilon;
    });
  }

  addToDatabase(destination: CityDto): void {
    if (!destination.name) return;

    if (this.isDuplicate(destination)) {
      this.toaster.warn('Este destino ya existe en tu base de datos.', 'Duplicado');
      return;
    }

    const input: CreateUpdateDestinationDTO = {
      name: destination.name || '',
      country: destination.country || '',
      poblation: (destination as any).population || 0,
      photoUrl: (destination as any).imageUrl || '',
      lastUpdate: new Date().toISOString(),
      coordinates: {
        latitude: (destination as any).latitude || 0,
        longitude: (destination as any).longitude || 0
      }
    };

    this.destinationService.create(input).subscribe({
      next: () => {
        this.toaster.success('Destino guardado exitosamente', 'Éxito');
        if (destination.name) this.savedItems.add(destination.name);
        this.localDestinations.push(this.normalizeCityData(input));
      },
      error: (err) => {
        console.error('❌ Error al guardar:', err);
        this.toaster.error('No se pudo guardar el destino.', 'Error');
      }
    });
  }

  onSearch(): void {
    this.searchParams.skipCount = 0;
    this.currentPage = 1;
    this.loadDestinations();
  }

  clearSearch(): void {
    this.searchParams.query = '';
    this.searchParams.country = '';
    this.onSearch();
  }

  onImageError(event: any): void {
    event.target.src = this.defaultImage;
  }

  formatCoordinates(latitude?: number, longitude?: number): string {
    if (!latitude || !longitude) return 'N/A';
    return `${Number(latitude).toFixed(4)}, ${Number(longitude).toFixed(4)}`;
  }

  formatPopulation(population?: number): string {
    if (!population) return 'N/A';
    return population.toLocaleString('es-ES');
  }

  openInMaps(destination: CityDto): void {
    const dest: any = destination;
    const lat = dest.latitude;
    const lng = dest.longitude;

    if (lat && lng) {
      const url = `https://www.google.com/maps/search/?api=1&query=${lat},${lng}`;
      window.open(url, '_blank');
    }
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.searchParams.skipCount = (page - 1) * this.searchParams.maxResultCount;
    this.loadDestinations();
  }

  getDestinationImage(imageUrl?: string): string {
    if (imageUrl && imageUrl.startsWith('http')) {
      return imageUrl;
    }
    return imageUrl ? environment.apis.default.url + imageUrl : this.defaultImage;
  }
}