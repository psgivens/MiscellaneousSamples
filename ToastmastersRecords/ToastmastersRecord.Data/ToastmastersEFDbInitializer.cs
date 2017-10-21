using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastmastersRecord.Data {
    internal class ToastmastersEFDbInitializer : DropCreateDatabaseIfModelChanges<ToastmastersEFDbContext> {
        protected override void Seed(ToastmastersEFDbContext context) {
            //var x = new RoleRequestEnvelopeEntity {
            //    StreamId = Guid.NewGuid(),
            //    TransactionId = Guid.NewGuid(),
            //    UserId = Guid.NewGuid().ToString(),
            //    DeviceId = Guid.NewGuid().ToString (),
            //    Version = 0,
            //    Id = Guid.NewGuid (),
            //    TimeStamp = DateTimeOffset.Now ,
            //    Event = ""
            //    };

            base.Seed(context);
        }
    }
}
