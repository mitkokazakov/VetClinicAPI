using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Pets = new HashSet<Pet>();
        }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string Town { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
