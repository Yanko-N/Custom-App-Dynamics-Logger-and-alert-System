using Domain.Common.List;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        public Task<PaginatedList<CustomLog>> GetLogsAsync(int apiKey, int skip, int take)
        {
            throw new NotImplementedException();
        }
    }
}
