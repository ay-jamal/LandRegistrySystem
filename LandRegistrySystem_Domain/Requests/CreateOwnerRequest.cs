using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class CreateOwnerRequest
    {
        [Required(ErrorMessage = "اسم المالك مطلوب")]
        public string FullName { get; set; }       // الاسم الكامل

        [Required(ErrorMessage = " الرقم الوطني مطلوب")]
        public string NationalId { get; set; }     // رقم الهوية الوطنية
        public string PhoneNumber { get; set; }
    }
}
