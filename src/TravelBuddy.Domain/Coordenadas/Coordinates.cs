using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBuddy.Coordenadas
{
    public class Coordinates
    {
        public required float Latitude { get; set; }
        public required float Longitude { get; set; }

        public Coordinates(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

    }
}
