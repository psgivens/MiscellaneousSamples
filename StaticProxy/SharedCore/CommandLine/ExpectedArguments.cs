using System.Collections.Generic;

namespace PhillipScottGivens.SharedCore
{
    public class ExpectedArguments
    {
        private readonly ExpectedArgument[] args;
        private readonly Dictionary<string , ExpectedArgument> arguments = new Dictionary<string, ExpectedArgument>();

        public ExpectedArguments(params ExpectedArgument[] args)
        {
            if (args == null)
                return;

            this.args = args;

            for (int index = 0; index < args.Length; index++)
            {
                ExpectedArgument argument = args[index];
                arguments.Add(argument.Key, argument);
            }
        }

        public string this[string argumentName]
        {
            get
            {
                return arguments[argumentName].Value;
            }
            set
            {
                arguments[argumentName].Value = value;
            }
        }

        public string this[int index]
        {
            get
            {
                return args[index].Value;
            }
            set
            {
                args[index].Value = value;
            }
        }

        public int Length
        {
            get
            {
                return args.Length;
            }
        }

        public string GetKey(int index)
        {
            return args[index].Key;
        }

        public string GetDescription(int index)
        {
            return args[index].Description;
        }
    }
}
