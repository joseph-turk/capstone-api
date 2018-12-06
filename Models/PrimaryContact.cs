using System;
using System.Collections.Generic;

namespace CapstoneApi.Models
{
    public class PrimaryContact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public virtual List<Registration> Registrations { get; set; }

        public override bool Equals(object obj)
        {
            Type objType = obj.GetType();
            bool typesEqual = this.GetType().Equals(objType);

            PrimaryContact item = null;
            if (typesEqual)
            {
                item = (PrimaryContact)obj;
            }

            if (item == null) return false;
            if (this.Id.Equals(item.Id)) return true;

            return this.Name.Equals(item.Name)
                && this.EmailAddress.Equals(item.EmailAddress)
                && this.PhoneNumber.Equals(item.PhoneNumber);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}