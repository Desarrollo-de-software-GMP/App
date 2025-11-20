using System;
using TravelBuddy.Common;
using Volo.Abp; 
using Volo.Abp.Domain.Entities.Auditing;


namespace TravelBuddy.Califications
{
    public class Calification : AuditedAggregateRoot<Guid>, IUserOwned
    {
    
        public required int punctuation { get; set; }
        public Guid UserId { get; set; }        // Requerido por IUserOwned
        public Guid DestinationId { get; set; }
        public string comment { get; set; }


        public Calification()
        {
        }

        public Calification(
            Guid id,
            Guid userId,
            Guid destinationId,
            int punctuation,
            string? comment = null)
            : base(id)
        {
            UserId = userId;
            DestinationId = destinationId;
            this.punctuation = punctuation;
            this.comment = comment;
        }
    }
}