using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersOfBabel {
    public interface INumberTranslator {
        string TranslateNumber(int number);
        string Language { get; }
    }
}
