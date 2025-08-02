using LandRegistrySystem_Domain.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } // New property (default: active)

        public Role Role { get; set; }

        public void Update(UpdateUserRequest updateUserRequest)
        {
            // Update properties if they are provided in the request

            if (!string.IsNullOrEmpty(updateUserRequest.Username))
                Username = updateUserRequest.Username;

            if (!string.IsNullOrEmpty(updateUserRequest.FullName))
                FullName = updateUserRequest.FullName;

            if (!string.IsNullOrEmpty(updateUserRequest.Phone))
                Phone = updateUserRequest.Phone;

            if (!string.IsNullOrEmpty(updateUserRequest.Adress))
                Adress = updateUserRequest.Adress;

            if (!string.IsNullOrEmpty(updateUserRequest.Email))
                Email = updateUserRequest.Email;

            if (updateUserRequest.RoleId.HasValue)
                RoleId = updateUserRequest.RoleId.Value;

            // Handle password update if a new password is provided
            if (!string.IsNullOrEmpty(updateUserRequest.Password))
            {
                // Hash the new password and update
                using var hmac = new System.Security.Cryptography.HMACSHA512();
                PasswordSalt = hmac.Key;  // Store the salt used for the password hash
                Password = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(updateUserRequest.Password)));
            }

            // Update IsActive status if provided
            if (updateUserRequest.IsActive.HasValue)
                IsActive = updateUserRequest.IsActive.Value;
        }
        public void UpdateStatus(UpdateUserStatusRequest userStatusToUpdate)
        {
            Id = userStatusToUpdate.Id;
            IsActive = userStatusToUpdate.IsActive;
        }
    }
}
