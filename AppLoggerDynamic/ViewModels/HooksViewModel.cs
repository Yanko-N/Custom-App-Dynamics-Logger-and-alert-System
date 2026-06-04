using Domain.Entities;

namespace AppLoggerDynamic.ViewModels
{
    public class HooksViewModel
    {
        public List<Service> Services { get; set; } = new();
        public int SelectedServiceId { get; set; }
        public List<Hook> Hooks { get; set; } = new();
    }

    public class HookFormViewModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Secret { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Service> Services { get; set; } = new();
    }
}
