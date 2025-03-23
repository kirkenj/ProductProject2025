namespace Exceptions
{
    public class CouldNotGetEnvironmentVariableException : Exception
    {
        public string EnvironmentVariableName { get; private set; }

        public override string Message => $"Couldn't get value from environment variable with name '{EnvironmentVariableName}'.";

        public CouldNotGetEnvironmentVariableException(string environmentVariableName) : base()
        {
            EnvironmentVariableName = environmentVariableName;
        }
    }
}
