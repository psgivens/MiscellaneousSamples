using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastmastersRecord.Data.Entities {
    public class MemberEntity {
        [Key]
        public virtual Guid Id { get; set; }
        public virtual int MemberId { get; set; }
        public virtual string FullName { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
