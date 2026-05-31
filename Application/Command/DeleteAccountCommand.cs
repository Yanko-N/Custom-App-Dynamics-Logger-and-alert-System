using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class DeleteAccountCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public DeleteAccountCommand(int id) => Id = id;
    }

    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<bool>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;

        public DeleteAccountCommandHandler(IAccountRepository accountRepository, ILogger<DeleteAccountCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _accountRepository.DeleteAccountAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(AccountErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting account {AccountId}", request.Id);
                return Result.Failure<bool>(AccountErrors.Unknown);
            }
        }
    }
}
