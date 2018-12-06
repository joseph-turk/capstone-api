using System;
using System.Collections.Generic;

namespace CapstoneApi.Models
{
    public class RegistrationViewModel
    {
        public Event Event { get; set; }
        public PrimaryContact PrimaryContact { get; set; }
        public List<Registrant> Registrants { get; set; }
    }
}