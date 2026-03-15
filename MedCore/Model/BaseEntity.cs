using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] Version { get; set; }
    }
}
