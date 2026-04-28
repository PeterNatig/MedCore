using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Model
{
    public abstract class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime LastModified { get; set; }

        public bool IsDeleted { get; set; }

        public byte[] Version { get; set; }
    }
}
