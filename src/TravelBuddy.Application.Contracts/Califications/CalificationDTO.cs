using System;
using Volo.Abp.Application.Dtos;

namespace TravelBuddy.Califications.Dtos
{
    // Asegúrate de que la clase sea PÚBLICA
    public class CalificationDTO : AuditedEntityDto<Guid>
    {
        public int punctuation { get; set; }
        public string? comment { get; set; }
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
    }
}