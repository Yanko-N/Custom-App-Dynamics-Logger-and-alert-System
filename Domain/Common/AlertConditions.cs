namespace Domain.Common
{
    /// <summary>
    /// Constants for alert condition types.
    /// </summary>
    public static class AlertConditions
    {
        public const string GreaterThan = "GreaterThan";
        public const string LessThan = "LessThan";
        public const string Equals_Symbol = "Equals";

        /// <summary>
        /// Gets the symbol representation of the condition.
        /// </summary>
        public static string GetSymbol(string condition)
        {
            switch (condition)
            {
                case GreaterThan:
                    return ">";
                case LessThan:
                    return "<";
                case Equals_Symbol:
                    return "=";
                default:
                    return "?";
            }
        }
        public static readonly string[] ValidConditions = { GreaterThan, LessThan, Equals_Symbol };
    }
}
