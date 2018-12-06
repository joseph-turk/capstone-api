using CapstoneApi.Models;

namespace CapstoneApi.Dtos
{
    public class EventDto
    {
        public Event Event { get; set; }
        public bool IsMyEvent { get; set; }
    }
}