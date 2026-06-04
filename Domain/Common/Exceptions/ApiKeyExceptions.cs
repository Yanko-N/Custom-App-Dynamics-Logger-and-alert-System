namespace Domain.Common.Exceptions
{
    public class ApiKeyLabelConflictException : Exception
    {
        public ApiKeyLabelConflictException(string label)
            : base($"An API key with the label '{label}' already exists for this account.")
        {
        }
    }
}
