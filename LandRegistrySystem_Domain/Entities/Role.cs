using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }  // مثال: "Super Admin", "HR Manager"

        // قائمة المستخدمين المرتبطين بهذا الدور
        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
