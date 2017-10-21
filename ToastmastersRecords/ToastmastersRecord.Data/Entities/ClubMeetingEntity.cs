using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastmastersRecord.Data.Entities {
    public class ClubMeetingEntity {
        [Key]
        public virtual Guid Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Theme { get; set; }
        public virtual int State { get; set; }
    }
}
