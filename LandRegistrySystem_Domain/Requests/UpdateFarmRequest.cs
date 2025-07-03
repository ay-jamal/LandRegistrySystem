using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class UpdateFarmRequest
    {
        [Required(ErrorMessage = " لم يتم العثور على المزرعة")]
        public int Id { get; set; }

        [Required(ErrorMessage = "رقم المزرعة مطلوب")]
        public string FarmNumber { get; set; }

        [Required]
        public double Area { get; set; }

        [Required(ErrorMessage = " يجب تحديد المشروع ")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "يجب تحديد مالك للمزرعة")]
        public int OwnerId { get; set; }

        // Boundaries
        public string North { get; set; }
        public string South { get; set; }
        public string East { get; set; }
        public string West { get; set; }
    }
}
