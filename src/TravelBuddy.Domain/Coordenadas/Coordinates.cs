using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBuddy.Coordenadas
{
    public class Coordinates(float latitude, float longitude)
    {
        public required float Latitude { get; set; } = latitude;
        public required float Longitude { get; set; } = longitude;

    } 

}
