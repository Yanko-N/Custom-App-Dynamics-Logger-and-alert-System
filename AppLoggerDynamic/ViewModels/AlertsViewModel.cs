using Domain.Common;
using Domain.Entities;

namespace AppLoggerDynamic.ViewModels
{
    public class AlertsViewModel
    {
        public List<Service> Services { get; set; } = new();
        public int SelectedServiceId { get; set; }
        public List<AlertStatusItem> AlertStatuses { get; set; } = new();
    }

    public class AlertStatusItem
    {
        public Alert Alert { get; set; } = null!;
        public int CurrentCount { get; set; }
        public bool IsViolating { get; set; }

        public string ConditionSymbol => AlertConditions.GetSymbol(Alert.Condition);

        public string RuleSummary =>
            $"Fire when {Alert.Level} count {ConditionSymbol} {Alert.ThresholdValue} in {Alert.WindowSeconds}s";

        public int ProgressPercent
        {
            get
            {
                if (Alert.ThresholdValue <= 0) return 100;
                var pct = (int)((CurrentCount / (double)Alert.ThresholdValue) * 100);
                return Math.Min(pct, 100);
            }
        }
    }

    public class AlertFormViewModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = "ERROR";
        public string Condition { get; set; } = AlertConditions.GreaterThan;
        public int ThresholdValue { get; set; } = 5;
        public int WindowSeconds { get; set; } = 60;
        public bool IsActive { get; set; } = true;
        public string? MessagePattern { get; set; }
        public List<Service> Services { get; set; } = new();
    }
}
