using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TravelBuddy.Destinations;
namespace TravelBuddy.Destinations
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private static readonly string apiKey = "1b87288382msh04081de1250362fp1acf94jsn6c66e7e31d14";
        private static readonly string baseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";

        public async Task<CitySearchResultDto> SearchCities(CitySearchRequestDTO request)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

            string url = $"{baseUrl}/cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit=10&sort=population";

            HttpResponseMessage response = await httpClient.GetAsync(url);

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


