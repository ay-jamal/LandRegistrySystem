using LandRegistrySystem_Domain.Requests;
using System;
using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace LandRegistrySystem_Domain.Entities
    {
        public class OrganizationInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public byte[] Logo { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public DateTime? UpdatedAt { get; set; }  // nullable لأن ممكن ما يتحدثش من البداية
             public string UpdatedByUserName { get; set; }
        public void Update(UpdateOrgRequest request, byte[]? logoBytes = null)
        {
            Name = request.Name;
            Phone = request.Phone;
            Email = request.Email;
            UpdatedAt = DateTime.Now;
            Address = request.Address;
            if (logoBytes != null && logoBytes.Length > 0)
                Logo = logoBytes;
        }


    }
}
