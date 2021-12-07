using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.DTO.Pets
{
    public class AddPetFormModel
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public string Breed { get; set; }

        public DateTime BirthDate { get; set; }

        
        public IFormFile Image { get; set; }
    }
}
