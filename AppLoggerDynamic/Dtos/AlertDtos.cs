namespace AppLoggerDynamic.Dtos
{
    public static class AlertDtos
    {
        public record CreateAlertRequest
        {
            public int ServiceId { get; init; }
            public string Name { get; init; } = string.Empty;
            public string Level { get; init; } = string.Empty;
            public string Condition { get; init; } = string.Empty;
            public int ThresholdValue { get; init; }
            public int WindowSeconds { get; init; } = 60;
        }

        public record UpdateAlertRequest
        {
            public string Name { get; init; } = string.Empty;
            public string Level { get; init; } = string.Empty;
            public string Condition { get; init; } = string.Empty;
            public int ThresholdValue { get; init; }
            public int WindowSeconds { get; init; }
            public bool IsActive { get; init; }
        }

        public record AlertResponse
        {
            public int Id { get; init; }
            public int ServiceId { get; init; }
            public string Name { get; init; } = string.Empty;
            public string Level { get; init; } = string.Empty;
            public string Condition { get; init; } = string.Empty;
            public int ThresholdValue { get; init; }
            public int WindowSeconds { get; init; }
            public bool IsActive { get; init; }
        }
    }
}
