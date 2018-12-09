using System;
using System.Collections.Generic;
using CapstoneApi.Models;

namespace CapstoneApi.Dtos
{
    public class RegistrantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool PhotoRelease { get; set; }
    }
}