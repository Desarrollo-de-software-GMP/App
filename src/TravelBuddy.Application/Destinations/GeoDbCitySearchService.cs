using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.Destinations;
using TravelBuddy.Application.Destinations;

namespace TravelBuddy.Destinations
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private static readonly string apiKey = "79f71dea3bmsh428d736660ceb5ap159e4bjsn2d9cfc576041";
        private static readonly string baseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private readonly HttpClient _httpClient;

        public GeoDbCitySearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CitySearchResultDto> SearchCities(CitySearchRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.PartialName))
            {
                return new CitySearchResultDto { Cities = new List<CityDto>() };
            }
            _httpClient.DefaultRequestHeaders.Clear(); 
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

            string url = $"{baseUrl}/cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit=10&sort=population";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();
                var cities = json.Data.Select(c => new CityDto
                {
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    Population= c.Population ?? 0,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude

                }).ToList();
                return new CitySearchResultDto { Cities = cities };
            }
            else
            {
                 throw new Exception($"Error fetching cities: {response.ReasonPhrase}");
            }

        }

    }
}


