namespace AppLoggerDynamic.Dtos
{
    public static class AccountDtos
    {
        public record CreateAccountRequest
        {
            public string Name { get; init; } = string.Empty;
        }

        public record CreateAccountResponse
        {
            public int Id { get; init; }
            public string Name { get; init; } = string.Empty;
        }

        public record UpdateAccountRequest
        {
            public string Name { get; init; } = string.Empty;
            public bool IsActive { get; init; }
        }

        public record AccountResponse
        {
            public int Id { get; init; }
            public string Name { get; init; } = string.Empty;
            public bool IsActive { get; init; }
            public DateTime CreatedAt { get; init; }
        }
    }
}