namespace Domain.Common.Exceptions
{
    public class AlertNameConflictException : Exception
    {
        public AlertNameConflictException(string name)
            : base($"An alert with the name '{name}' already exists for this service.")
        {
        }
    }
}
