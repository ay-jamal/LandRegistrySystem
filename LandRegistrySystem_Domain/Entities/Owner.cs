using LandRegistrySystem_Domain.Requests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class Owner
    {
        public int Id { get; set; }
        public string FullName { get; set; } 
        public string NationalId { get; set; }     
        public string PhoneNumber { get; set; }
        public List<Farm> Farms { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsProtected { get; set; } // 👈 خاصية حماية المالك الافتراضي
        public DateTime UpdatedAt { get; set; }
        public string UpdatedByUserName { get; set; }
        public void Update(UpdateOwnerRequest owner, string userName)
        {
            // لا تسمح بتعديل المالك الافتراضي
            if (!IsProtected)
            {
                FullName = owner.FullName;
                NationalId = owner.NationalId;
                PhoneNumber = owner.PhoneNumber;
                UpdatedAt = DateTime.Now;
                UpdatedByUserName = userName;

            }
            // ممكن ترجع رسالة أو ترمي استثناء إذا حاول أحد التعديل!
        }

    }
}
