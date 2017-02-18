using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PhillipScottGivens.SharedCore
{
    public class CommandLineArguments
    {
        #region Fields
        private string[] args;
        private Dictionary<string, CommandLineFlag> flagMap = new Dictionary<string, CommandLineFlag>();
        private readonly ExpectedArguments expectedArguments;
        #endregion

        public CommandLineArguments(Assembly executable, string[] args, params CommandLineFlag[] flags)
            : this(executable, args, new ExpectedArguments(), flags) { }

        public CommandLineArguments(Assembly executable, string[] args, ExpectedArguments expectedArguments, params CommandLineFlag[] flags)
        {
            this.args = args;
            this.expectedArguments = expectedArguments;
            int expectedArgumentIndex = 0;

            foreach (var flag in flags)
                flagMap.Add(flag.LongForm, flag);
            
            try
            {
                #region Handle found flag.
                Func<string[], CommandLineFlag, int, int> handleFoundFlag = (arguments, flag, index) =>
                {
                    if (flag == null)
                        throw new CommandLineUsageException(string.Format("unsupported flag {0} found.", arguments[index]));

                    flag.MarkFound();
                    if (flag.ExpectsParameter)
                    {
                        // TODO: Handle index out of range exception?
                        flag.Values.Add(arguments[++index]);
                    }
                    return index;
                };
                #endregion

                #region Find flags.
                for (int index = 0; index < args.Length; index++)
                {
                    string argument = args[index];
                    if (argument.StartsWith("--"))
                    {
                        string longArgument = argument.Substring(2);
                        CommandLineFlag flag = flags.FirstOrDefault(f => f.LongForm == longArgument);
                        index = handleFoundFlag(args, flag, index);
                    }
                    else if (argument.StartsWith("-")
                        || argument.StartsWith("/")
                        || argument.StartsWith("\\"))
                    {
                        string shortArgument = argument.Substring(1);
                        CommandLineFlag flag = flags.FirstOrDefault(f => f.ShortForm == shortArgument);
                        index = handleFoundFlag(args, flag, index);
                    }
                    else
                    {
                        expectedArguments[expectedArgumentIndex++] = argument;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                StringBuilder usageMessage = new StringBuilder("Usage: \n");
                usageMessage.Append(executable.FullName + " ");
                for (int index = 0; index < expectedArguments.Length; index++)
                {
                    usageMessage.Append(String.Format("<{0}> ", expectedArguments.GetKey(index)));
                }
                usageMessage.AppendLine();
                usageMessage.AppendLine("Argument Details:");
                for (int index = 0; index < expectedArguments.Length; index++)
                {
                    usageMessage.Append(String.Format("<{0}> ", expectedArguments.GetKey(index)));
                    usageMessage.AppendLine("  " + expectedArguments.GetDescription(index));
                }
                usageMessage.AppendLine("Expected Flags:");
                foreach (CommandLineFlag flag in flags)
                    usageMessage.AppendLine(string.Format("-{0}  --{1}  {2}  {3}",
                        flag.ShortForm,
                        flag.LongForm,
                        flag.ExpectsParameter ? String.Format("<{0}>", flag.ExpectedParameter) : string.Empty,
                        flag.Description));
                throw new CommandLineUsageException(usageMessage.ToString(), e);
            }
        }

        public bool Found(string flagLongName)
        {
            return flagMap[flagLongName].WasFound;
        }

        public CommandLineFlag this[string flagLongName]
        {
            get
            {
                return flagMap[flagLongName];
            }
        }

        public string this[int index]
        {
            get
            {
                return expectedArguments[index];
            }
        }
    }
}
