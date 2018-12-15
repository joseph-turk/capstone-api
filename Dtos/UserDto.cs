using System;

namespace CapstoneApi.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public bool IsAdmin { get; set; }
    }
}