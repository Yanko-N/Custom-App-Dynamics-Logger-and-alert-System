namespace Domain.Common.Exceptions
{
    public class ServiceNameConflictException : Exception
    {
        public ServiceNameConflictException(string name)
            : base($"A service with the name '{name}' already exists for this account.")
        {
        }
    }
}
