using System;
using System.Collections.Generic;
using CapstoneApi.Models;

namespace CapstoneApi.Dtos
{
    public class RegistrationDto
    {
        public Event Event { get; set; }
        public PrimaryContact PrimaryContact { get; set; }
        public List<RegistrantDto> Registrants { get; set; }
    }
}