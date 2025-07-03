using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class FarmBranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }  // يمكنك استخدام string أو enum

        public string Description { get; set; }
        public double Area { get; set; }

        public BoundaryDto Boundaries { get; set; }
        public OwnerDto Owner { get; set; }

        public List<FarmDocumentDto> Documents { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
