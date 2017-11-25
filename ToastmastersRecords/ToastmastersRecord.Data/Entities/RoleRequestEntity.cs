﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastmastersRecord.Data.Entities {
    public class RoleRequestEntity {
        [Key]
        public virtual Guid Id { get; set; }
        public virtual int State { get; set; }
        public virtual Guid MessageId { get; set; }
        public virtual string Brief { get; set; }
        public virtual IList<RoleRequestDate> Dates { get; set; }
        public virtual Guid MemberId { get; set; }
    }
    public class RoleRequestDate {
        [Key, Column(Order = 1)]
        public virtual Guid RoleRequestId { get; set; }
        public virtual RoleRequestEntity RoleRequest { get; set; }
        [Key, Column(Order = 2, TypeName = "Date")]
        [DataType(DataType.Date)]
        public virtual DateTime Date { get; set; }
    }
}
