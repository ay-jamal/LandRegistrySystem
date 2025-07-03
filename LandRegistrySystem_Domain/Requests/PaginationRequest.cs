using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class PaginationRequest
    {
        public string SearchValue { get; set; } = String.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public bool SkipPaged { get; set; } = false; 

    }
}
