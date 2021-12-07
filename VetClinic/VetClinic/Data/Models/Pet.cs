using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Models
{
    public class Pet
    {
        public Pet()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Visitations = new HashSet<Visitation>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        public string Kind { get; set; }

        [Required]
        [MaxLength(30)]
        public string Breed { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Image")]
        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public virtual ICollection<Visitation> Visitations { get; set; }
    }
}
