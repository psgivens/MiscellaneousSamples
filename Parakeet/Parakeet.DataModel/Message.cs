using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Parakeet.DataModel
{
    public class Message : Entity, ICloneable
    {
        public virtual Subject Subject { get; set; }
        public virtual string Caption { get; set; }
        public virtual Status Status { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime ReviewedAt { get; set; }

        public object Clone()
        {
            return new Message
            {
                ID = Guid.NewGuid(),
                Subject = this.Subject,
                Caption = this.Caption,
                Status = this.Status,
                CreatedAt = this.CreatedAt,
                ReviewedAt = this.ReviewedAt
            };
        }
    }
}
