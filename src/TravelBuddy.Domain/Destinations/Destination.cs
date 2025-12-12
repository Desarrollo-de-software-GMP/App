using System;
using System.Diagnostics.CodeAnalysis; 
using TravelBuddy.Coordenadas;
using TravelBuddy.Coordenadas.TravelBuddy.Coordenadas;
using Volo.Abp.Domain.Entities.Auditing;

namespace TravelBuddy.Destinations
{
    public class Destination : AuditedAggregateRoot<Guid>
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required int Poblation { get; set; }
        public Coordinates Coordinates { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public Destination() { }

        // Agrega este atributo [SetsRequiredMembers]
        [SetsRequiredMembers]
        public Destination(Guid id, string name, string country, int poblation, Coordinates coordinates, string photoURL, DateTime lastUpdate) : base(id)
        {
            this.Name = name;
            this.Country = country;
            this.Poblation = poblation;
            this.Coordinates = coordinates;
            this.PhotoUrl = photoURL;
            this.LastUpdate = lastUpdate;
        }
    }
}