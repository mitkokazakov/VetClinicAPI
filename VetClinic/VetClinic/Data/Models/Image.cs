using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Models
{
    public class Image
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        [Required]
        public string Id { get; set; }

        public bool IsApproved { get; set; } = false;

        [Required]
        public string Extension { get; set; }

        [Required]
        [ForeignKey("Pet")]
        public string PetId { get; set; }
        
        public virtual Pet Pet { get; set; }
    }
}
