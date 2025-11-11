using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBuddy.Application.Destinations
{
    class GeoDbResponse
    {
        public List<GeoDbCity> Data { get; set; } = new();
    }
}
