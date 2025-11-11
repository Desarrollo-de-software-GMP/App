namespace TravelBuddy.Destinations
{
    public class CityDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? PhotoUrl { get; set; }
    }
}