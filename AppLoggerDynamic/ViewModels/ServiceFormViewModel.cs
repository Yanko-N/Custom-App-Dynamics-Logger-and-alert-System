using Domain.Entities;

namespace AppLoggerDynamic.ViewModels
{
    public class ServiceFormViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
