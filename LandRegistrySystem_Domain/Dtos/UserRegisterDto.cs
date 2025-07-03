using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 أحرف.")]
        public string Password { get; set; }

        public string FullName { get; set; }


        public string Email { get; set; }

        public string Phone { get; set; }

        public string Adress { get; set; }

        public bool IsActive { get; set; }

        public int RoleId { get; set; } // RoleId from the Roles table
    }
}
