using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.DTO.Visitations
{
    public class VisitationViewModel
    {
        public string Id { get; set; }
        public string Date { get; set; }

        public string Reason { get; set; }

        public string Description { get; set; }
    }
}
