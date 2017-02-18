using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Parakeet.Data.UnitTests
{
    [TestClass]
    public class UsersTests
    {
        [TestMethod]
        public void AddUser()
        {
            int id = 0;
            var userName = string.Format("Tom {0} Chancy", id);
            using (var context = new ParakeetModelContainer())
            {
                var user = context.Users.Add(new User
                {
                    UserName = userName,
                });
                
                context.SaveChanges();
                id = user.Id;
            }
            using (var context = new ParakeetModelContainer())
            {
                var user = (from u in context.Users
                            where u.Id == id
                            select u)
                            .FirstOrDefault();
                
                Assert.IsNotNull(user);

                var users = context.Users.ToList();
                foreach (var u in users)
                    context.Users.Remove(u);

                context.SaveChanges();
            }
        }
    }
}
