using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Data.UnitTests
{
    public static class ParakeetSamples
    {
        public static void PopulateSample1(this ParakeetModelContainer context)
        {
            var tomChancy = context.Users.Add(new User
            {
                UserName = "Tom Chancy",
            });
            var task1 = context.Tasks.Add(new Task
            {
                AssignedUser = tomChancy,
                Title = "Get caught",
                Description = "Wait for little ... to come get you.",
            });
        }
    }
}
