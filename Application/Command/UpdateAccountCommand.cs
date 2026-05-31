using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class UpdateAccountCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<bool>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<UpdateAccountCommandHandler> _logger;

        public UpdateAccountCommandHandler(IAccountRepository accountRepository, ILogger<UpdateAccountCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<bool>(AccountErrors.InvalidName);
                }

                var success = await _accountRepository.UpdateAccountAsync(request.Id, request.Name, request.IsActive, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(AccountErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Domain.Common.Exceptions.AccountAlreadyExistsException)
            {
                return Result.Failure<bool>(AccountErrors.NameTaken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating account {AccountId}", request.Id);
                return Result.Failure<bool>(AccountErrors.Unknown);
            }
        }
    }
}
