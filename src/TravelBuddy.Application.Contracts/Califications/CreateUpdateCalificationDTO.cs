using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBuddy.Califications.Dtos
{
  
    public class CreateUpdateCalificationDTO
    {
        [Required]
        [Range(1, 5)] 
        public int punctuation { get; set; }

        [StringLength(1000)] 
        public string? comment { get; set; }

        [Required]
        public Guid DestinationId { get; set; }

    }
}