using LandRegistrySystem_Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class Farm
    {
        public int Id { get; set; }
        public string FarmNumber {  get; set; }
        public double Area { get; set; } 
        public byte[] BarcodeImage { get; set; }

        public FarmBoundaries Boundaries { get; set; } 

        public int ProjectId { get; set; }
        public Project Project { get; set; } 

        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
        public ICollection<FarmDocument> Documents { get; set; } = new List<FarmDocument>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }  // nullable لأن ممكن ما يتحدثش من البداية
        public string UpdatedByUserName { get; set; }

        public void Update(UpdateFarmRequest dto, string updatedBy)
        {
            Area = dto.Area;
            ProjectId = dto.ProjectId;
            OwnerId = dto.OwnerId;
            FarmNumber = dto.FarmNumber;
            UpdatedAt = DateTime.Now;

            if (Boundaries != null)
            {
                Boundaries.North = dto.North;
                Boundaries.South = dto.South;
                Boundaries.East = dto.East;
                Boundaries.West = dto.West;
            }
            else
            {
                Boundaries = new FarmBoundaries
                {
                    North = dto.North,
                    South = dto.South,
                    East = dto.East,
                    West = dto.West,
                    FarmId = this.Id
                };
            }
        }

}
}
