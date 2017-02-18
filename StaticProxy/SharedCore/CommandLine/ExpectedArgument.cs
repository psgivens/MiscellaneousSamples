
namespace PhillipScottGivens.SharedCore
{
    public class ExpectedArgument
    {
        public string Key { get; private set; }
        public string Description { get; private set; }
        public string Value { get; set; }

        public ExpectedArgument(string key, string Description)
        {
            this.Key = key;
            this.Description = Description;
        }
    }
}
