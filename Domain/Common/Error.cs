using Domain.Common.Enums;

namespace Domain.Common
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string code, string message, ErrorType type)
        {
            Code = code;
            Message = message;
            Type = type;
        }

        public static Error Unknown(string code, string message) =>
            new(code, message, ErrorType.Unknown);

        public static Error NotFound(string code, string message) =>
            new(code, message, ErrorType.NotFound);
        public static Error Validation(string code, string message) =>
            new(code, message, ErrorType.Validation);
        public static Error Conflict(string code, string message) =>
            new(code, message, ErrorType.Conflict);
        public static Error Unauthorized(string code, string message) =>
            new(code, message, ErrorType.Unauthorized);
        public static Error Forbidden(string code, string message) =>
            new(code, message, ErrorType.Forbidden);
        public static Error Failure(string code, string message) =>
            new(code, message, ErrorType.Failure);
        public static Error BadRequest(string code, string message) =>  
            new(code, message, ErrorType.BadRequest);
    }
}