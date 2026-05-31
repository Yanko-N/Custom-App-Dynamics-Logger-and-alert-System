namespace AppLoggerDynamic.Dtos
{
    public static class LogDtos
    {
        public record IngestLogRequest
        {
            public int ServiceId { get; init; }
            public string Level { get; init; } = string.Empty;
            public string Message { get; init; } = string.Empty;
            public string? StackTrace { get; init; }
            public Guid? TraceId { get; init; }
        }

        public record LogResponse
        {
            public long Id { get; init; }
            public int ServiceId { get; init; }
            public DateTime Timestamp { get; init; }
            public string Level { get; init; } = string.Empty;
            public Guid TraceId { get; init; }
            public string Message { get; init; } = string.Empty;
            public string? StackTrace { get; init; }
        }
    }
}
