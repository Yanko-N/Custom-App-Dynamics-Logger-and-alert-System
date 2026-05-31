namespace AppLoggerDynamic.Dtos
{
    public static class ServiceDtos
    {
        public record CreateServiceRequest
        {
            public string Name { get; init; } = string.Empty;
            public string Environment { get; init; } = string.Empty;
            public string Version { get; init; } = string.Empty;
        }

        public record UpdateServiceRequest
        {
            public string Name { get; init; } = string.Empty;
            public string Environment { get; init; } = string.Empty;
            public string Version { get; init; } = string.Empty;
        }

        public record ServiceResponse
        {
            public int Id { get; init; }
            public int AccountId { get; init; }
            public string Name { get; init; } = string.Empty;
            public string Environment { get; init; } = string.Empty;
            public string Version { get; init; } = string.Empty;
            public DateTime RegisteredAt { get; init; }
        }
    }
}
