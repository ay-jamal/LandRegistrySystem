﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class OwnerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }     // رقم الهوية أو الرقم الوطني
        public string PhoneNumber { get; set; }    // رقم الهاتف

        public int FarmsCount { get; set; }         // عدد المزارع المملوكة

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string UpdatedByUser { get; set; }

    }
}
