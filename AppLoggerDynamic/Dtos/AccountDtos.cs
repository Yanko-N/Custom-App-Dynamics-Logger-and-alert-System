namespace AppLoggerDynamic.Dtos
{
    public static class AccountDtos
    {

        public record CreateAccountRequest
        {
            public string Name { get; set; }
        }

        public record CreateAccountResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
