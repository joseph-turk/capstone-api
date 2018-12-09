using System;
using System.Collections.Generic;
using CapstoneApi.Dtos;

namespace CapstoneApi.Models
{
    public class RegistrationViewModel
    {
        public Event Event { get; set; }
        public PrimaryContact PrimaryContact { get; set; }
        public List<RegistrantDto> Registrants { get; set; }
    }
}