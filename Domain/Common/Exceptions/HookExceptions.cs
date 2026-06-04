namespace Domain.Common.Exceptions
{
    public class HookNameConflictException : Exception
    {
        public HookNameConflictException(string name)
            : base($"A hook with the name '{name}' already exists for this service.")
        {
        }
    }
}
