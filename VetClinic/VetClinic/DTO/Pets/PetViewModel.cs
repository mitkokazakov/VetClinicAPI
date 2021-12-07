using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.DTO.Pets
{
    public class PetViewModel
    {
        public string PetId { get; set; }

        public string Name { get; set; }

        public string Kind { get; set; }

        public string Breed { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age => this.CalculateAge(this.BirthDate);

        public string ImageId { get; set; }

        private int CalculateAge(DateTime date)
        {
            int age = 0;
            age = DateTime.Now.Subtract(date).Days;
            age = age / 365;
            return age;
        }
    }
}
