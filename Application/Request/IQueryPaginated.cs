using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    internal interface IQueryPaginated
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        
    }
}
