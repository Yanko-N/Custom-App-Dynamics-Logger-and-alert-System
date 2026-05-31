using Domain.Common.List;
using Domain.Entities;

namespace AppLoggerDynamic.ViewModels
{
    public class LogsViewModel
    {
        public List<Service> Services { get; set; } = new();
        public int SelectedServiceId { get; set; }
        public string Level { get; set; } = string.Empty;
        public PaginatedList<CustomLog>? Logs { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = 50;
    }
}
