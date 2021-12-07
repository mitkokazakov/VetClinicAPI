using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.DTO.Pets
{
    public class ChangePetFormModel
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public string Breed { get; set; }

        [AllowNull]
        public IFormFile Image { get; set; }
    }
}
