using System;
using System.Collections.Generic;

namespace CapstoneApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsAdmin { get; set; }

        public virtual List<Event> Events { get; set; }
    }
}