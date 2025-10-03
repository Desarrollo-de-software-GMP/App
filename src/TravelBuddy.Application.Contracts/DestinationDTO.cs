using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using System.ComponentModel.DataAnnotations;


namespace TravelBuddy
{
    public class DestinationDTO : AuditedEntityDto<Guid>
    {
        public required string Name { get; set; }
        [Required]
        public required string Country { get; set; }
        public int Poblation { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime LastUpdate { get; set; }
        public CoordinatesDTO Coordinates { get; set; }

    }
}
