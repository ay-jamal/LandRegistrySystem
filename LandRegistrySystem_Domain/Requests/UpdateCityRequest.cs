using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class UpdateCityRequest
    {
        [Required(ErrorMessage = " لم يتم العثور على المدينة")]
        public int Id { get; set; }

        [Required(ErrorMessage = " رقم المدينة مطلوب")]

        public string CityNumber { get; set; }

        [Required(ErrorMessage = " اسم المدينة مطلوب")]

        public string Name { get; set; }
    }
}
