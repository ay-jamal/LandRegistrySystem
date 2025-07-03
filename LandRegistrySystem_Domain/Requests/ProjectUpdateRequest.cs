using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class ProjectUpdateRequest
    {
        [Required(ErrorMessage = "لم يتم العثور على المشروع")]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "رقم المشروع مطلوب ")]
        public string ProjectNumber { get; set; }

        [Required(ErrorMessage = "يجب تحديد مدينة ")]
        public int CityId { get; set; }

    }
}
