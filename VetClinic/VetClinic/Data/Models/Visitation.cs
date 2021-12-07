using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Models
{
    public class Visitation
    {
        public Visitation()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public string PetId { get; set; }

        public virtual Pet Pet { get; set; }
    }
}
