using System;
using System.Collections.Generic;
using CapstoneApi.Models;

namespace CapstoneApi.Dtos
{
    public class EventDto
    {
        public Event Event { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Capacity { get; set; }
        public Guid ImageId { get; set; }
        public string ImageExtension { get; set; }
        public bool IsMyEvent { get; set; }
        public List<Registration> Registrations { get; set; }
        public int RegistrationCount { get; set; }
        public int WaitListCount { get; set; }
    }
}