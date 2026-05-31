namespace AppLoggerDynamic.Dtos
{
    public static class HookDtos
    {
        public record CreateHookRequest
        {
            public int ServiceId { get; init; }
            public string Name { get; init; } = string.Empty;
            public string Url { get; init; } = string.Empty;
            public string? Secret { get; init; }
        }

        public record UpdateHookRequest
        {
            public string Name { get; init; } = string.Empty;
            public string Url { get; init; } = string.Empty;
            public string? Secret { get; init; }
            public bool IsActive { get; init; }
        }

        public record HookResponse
        {
            public int Id { get; init; }
            public int ServiceId { get; init; }
            public string Name { get; init; } = string.Empty;
            public string Url { get; init; } = string.Empty;
            public bool IsActive { get; init; }
            public DateTime CreatedAt { get; init; }
        }
    }
}
