using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy
{
    public class DestinationDTO : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Poblation { get; set; }
    }
}
