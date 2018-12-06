using System;
using System.Collections.Generic;

namespace CapstoneApi.Models
{
    public class Registrant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual List<Registration> Registrations { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Registrant;

            if (item == null) return false;

            return this.Id.Equals(item.Id) || this.Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}