using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp {
    public class ActionableModel : ITreeAdaptor {
        System.Collections.IEnumerable ITreeAdaptor.GetChildren(object parent) {
            yield return new { Name = "Fun in the sun" };
            yield return new { Name = "The night begins" };
            yield return new { Name = "A new generation" };
        }

        bool ITreeAdaptor.HasChildren(object parent) {
            return true;
        }
    }

    public enum ActionType {
        ResultArea = 1,
        Project = 2,
        Task = 3,
        Step = 4
    }

   
}
