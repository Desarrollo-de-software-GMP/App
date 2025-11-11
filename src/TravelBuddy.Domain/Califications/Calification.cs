using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using TravelBuddy.Coordenadas;

namespace TravelBuddy.Califications
{
    internal class Calification : AuditedAggregateRoot<Guid>
    {
        public required DateTime createdDate { get; set; }
        public required DateTime updatedDate { get; set; }
        public required int punctuation { get; set; }
        public string ? comment { get; set; }
    }
}
