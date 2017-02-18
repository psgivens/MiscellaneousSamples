using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.SharedCore
{
    public class CommandLineFlag
    {
        public CommandLineFlag(string shortForm, string longForm, string description)
            : this(shortForm, longForm, null, description, false) { }

        public CommandLineFlag(string shortForm, string longForm, string expectedParameter, string description)
            : this(shortForm, longForm, expectedParameter, description, false) { }

        public CommandLineFlag(string shortForm, string longForm, string expectedParameter, string description, bool allowMultiple)
        {
            this.ShortForm = shortForm;
            this.LongForm = longForm;
            this.ExpectedParameter = expectedParameter;
            this.ExpectsParameter = !string.IsNullOrEmpty(expectedParameter);
            this.Description = description;
            this.AllowMultiple = allowMultiple;
            this.Values = new List<string>();
        }

        public string ShortForm { get; private set; }
        public string LongForm { get; private set; }
        public bool ExpectsParameter { get; private set; }
        internal string ExpectedParameter { get; private set; }
        public string Description { get; private set; }
        public List<string> Values { get; private set; }
        internal bool AllowMultiple { get; private set; }
        public bool WasFound { get; private set; }
        internal void MarkFound() { WasFound = true; }
    }
}
