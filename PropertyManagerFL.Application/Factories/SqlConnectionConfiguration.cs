namespace PropertyManagerFL.Application.Factories
{
    public class SqlConnectionConfiguration : ISqlConnectionConfiguration
    {
        public SqlConnectionConfiguration(string value) => Value = value;
        public string Value { get; }
    }
}
