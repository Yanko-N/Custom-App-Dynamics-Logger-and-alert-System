namespace AppLoggerDynamic.Dtos
{
    public static class ApiKeyDtos
    {
        public record CreateApiKeyRequest
        {
            public int AccountId { get; init; }
            public string Label { get; init; } = string.Empty;
            public DateTime? ExpiresAt { get; init; }
        }

        public record CreateApiKeyResponse
        {
            public int Id { get; init; }
            public string Label { get; init; } = string.Empty;
            public string RawKey { get; init; } = string.Empty;
        }

        public record ApiKeyResponse
        {
            public int Id { get; init; }
            public int AccountId { get; init; }
            public string Label { get; init; } = string.Empty;
            public bool IsActive { get; init; }
            public DateTime CreatedAt { get; init; }
            public DateTime? ExpiresAt { get; init; }
            public DateTime? LastUsedAt { get; init; }
            public string KeyHashPreview { get; init; } = string.Empty;
        }
    }
}
