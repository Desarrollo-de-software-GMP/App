using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using TravelBuddy.Common;




namespace TravelBuddy.Califications
{
    public class Calification : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public required DateTime createdDate { get; set; }
        public required DateTime updatedDate { get; set; }
        public required int punctuation { get; set; }
        public string ? comment { get; set; }

        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }

        public Calification() { }

        public Calification(Guid id, DateTime createdDate, DateTime updatedDate, int punctuation, string? comment) : base(id) {
            this.createdDate = createdDate;
            this.updatedDate = updatedDate;
            this.punctuation = punctuation;
            this.comment = comment;
        }
    }
}
