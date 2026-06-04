using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;

namespace Application.Command
{
    public class CreateAccountCommand : IRequest<Result<int>>
    {
        public string Name { get; set; }
    }

    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<int>>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Result<int>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<int>(AccountErrors.InvalidName);
                }

                var accountId = await _accountRepository.CreateAccountAsync(request.Name, cancellationToken);
                if (accountId == null)
                {
                    return Result.Failure<int>(AccountErrors.ErrorWhileSaving);
                }

                return Result.Success(accountId.Value);
            }
            catch (Domain.Common.Exceptions.AccountNameConflictException)
            {
                return Result.Failure<int>(AccountErrors.NameTaken);
            }
            catch (Exception)
            {
                return Result.Failure<int>(AccountErrors.Unknown);
            }
        }
    }
}
