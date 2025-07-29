using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class FarmDocument
    {
        public int Id { get; set; }

        public int FarmId { get; set; }
        public Farm Farm { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }  // nullable لأن ممكن ما يتحدثش من البداية
        public string UpdatedByUserName { get; set; }


    }
}
