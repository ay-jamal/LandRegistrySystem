using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class UpdateOwnerRequest
    {
        public int Id { get; set; }
        public string FullName { get; set; }       // الاسم الكامل
        public string NationalId { get; set; }     // رقم الهوية الوطنية
        public string PhoneNumber { get; set; }
    }
}
