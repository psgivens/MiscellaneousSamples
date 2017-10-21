using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastmastersRecord.Data.Entities {
    public class MemberMessageEntity {
        [Key]
        public virtual Guid Id { get; set; }
        public virtual string Message { get; set; }
        public virtual Guid MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }
    }
}
