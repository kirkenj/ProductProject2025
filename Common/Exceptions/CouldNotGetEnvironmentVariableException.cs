namespace Exceptions
{
    public class CouldNotGetEnvironmentVariableException : Exception
    {
        public string EnvironmentVariableName { get; private set; }
        public string ExpectedTypeName { get; private set; }

        public override string Message => $"Couldn't get {ExpectedTypeName} from environment variable with name '{EnvironmentVariableName}'.";

        public CouldNotGetEnvironmentVariableException(string environmentVariableName, string expectedTypeName = "string") : base()
        {
            EnvironmentVariableName = environmentVariableName;
            ExpectedTypeName = expectedTypeName;
        }
    }
}
