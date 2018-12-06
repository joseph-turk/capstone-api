using System;
using System.Collections.Generic;

namespace CapstoneApi.Models
{
    public class Registration
    {
        public Guid Id { get; set; }
        public virtual Event Event { get; set; }
        public virtual PrimaryContact PrimaryContact { get; set; }
        public virtual Registrant Registrant { get; set; }
        public bool HasPhotoRelease { get; set; }
        public bool IsWaitList { get; set; }
    }
}