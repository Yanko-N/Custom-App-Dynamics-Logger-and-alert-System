namespace Domain.Common
{
    /// <summary>
    /// Constants for environment CSS class mappings.
    /// </summary>
    public static class EnvironmentClasses
    {
        public const string Production = "env-production";
        public const string Staging = "env-staging";
        public const string Development = "env-development";
        public const string Default = "env-default";

        /// <summary>
        /// Gets the CSS class for the given environment.
        /// </summary>
        public static string GetClass(string? environment)
        {
            if (string.IsNullOrEmpty(environment))
                return Default;

            var env = environment.ToLower();
            switch (env)
            {
                case "production":
                    return Production;
                case "staging":
                    return Staging;
                case "development":
                case "dev":
                    return Development;
                default:
                    return Default;
            }
        }
    }
}
