using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetAccountsQuery : IRequest<Result<IEnumerable<Account>>>
    {
    }

    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, Result<IEnumerable<Account>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAccountsQueryHandler> _logger;

        public GetAccountsQueryHandler(IAccountRepository accountRepository, ILogger<GetAccountsQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Account>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await _accountRepository.GetAllAccountsAsync(cancellationToken);
                _logger.LogInformation("Fetched all accounts");
                return Result.Success(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching all accounts");
                return Result.Failure<IEnumerable<Account>>(AccountErrors.Unknown);
            }
        }
    }
}
