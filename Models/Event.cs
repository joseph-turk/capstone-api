using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CapstoneApi.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Capacity { get; set; }
        public Guid ImageId { get; set; }
        public string ImageExtension { get; set; }

        public virtual List<Registration> Registrations { get; set; }
        public virtual User CreatedBy { get; set; }
    }
}