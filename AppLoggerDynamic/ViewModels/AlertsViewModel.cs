using Domain.Entities;

namespace AppLoggerDynamic.ViewModels
{
    public class AlertsViewModel
    {
        public List<Service> Services { get; set; } = new();
        public int SelectedServiceId { get; set; }
        public IEnumerable<Alert> Alerts { get; set; } = Enumerable.Empty<Alert>();
    }

    public class AlertFormViewModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int ThresholdValue { get; set; }
        public int WindowSeconds { get; set; } = 60;
        public bool IsActive { get; set; } = true;
        public List<Service> Services { get; set; } = new();
    }
}
