using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetClinic.DTO.Users
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
