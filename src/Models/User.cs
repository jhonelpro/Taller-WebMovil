using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Birth_Date { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        //EtityFramework relationship
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}