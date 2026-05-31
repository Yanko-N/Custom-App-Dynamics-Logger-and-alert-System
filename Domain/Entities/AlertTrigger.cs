using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AlertTrigger
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Alert))]
        public int AlertId { get; set; }
        public virtual Alert? Alert { get; set; }

        public DateTime FiredAt { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; }

        // Navigation
        public virtual ICollection<HookEvent> HookEvents { get; set; }
    }
}