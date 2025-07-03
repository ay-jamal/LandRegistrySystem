using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class UpdateUserStatusRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
