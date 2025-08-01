﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string Password { get; set; }
    }
}
