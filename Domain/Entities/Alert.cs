using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Level { get; set; }

        [Required]
        [StringLength(50)]
        public string Condition { get; set; }

        public int ThresholdValue { get; set; }

        public int WindowSeconds { get; set; } = 60;

        [StringLength(200)]
        public string? MessagePattern { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<AlertTrigger> Triggers { get; set; }
    }
}