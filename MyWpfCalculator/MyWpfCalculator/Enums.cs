using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfCalculator {
    public enum Operator {
        Add,
        Multiply,
        Subtract,
        Divide,
        Reciprocal,
        Percent,
        Invert,
        Negate,
        Radical,
    }

    public enum Control {
        Clear,
        Back,
        Calculate,
        ClearE,
    }
}
