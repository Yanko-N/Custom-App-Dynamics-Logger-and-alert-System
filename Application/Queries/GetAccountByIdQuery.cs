using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetAccountByIdQuery : IRequest<Result<Account>>
    {
        public int Id { get; init; }

        public GetAccountByIdQuery(int id) => Id = id;
    }

    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<Account>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAccountByIdQueryHandler> _logger;

        public GetAccountByIdQueryHandler(IAccountRepository accountRepository, ILogger<GetAccountByIdQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Result<Account>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(request.Id, cancellationToken);

                if (account == null)
                {
                    _logger.LogWarning("Account {AccountId} not found", request.Id);
                    return Result.Failure<Account>(AccountErrors.NotFound(request.Id));
                }

                return Result.Success(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching account {AccountId}", request.Id);
                return Result.Failure<Account>(AccountErrors.Unknown);
            }
        }
    }
}
