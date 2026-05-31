using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class HookEvent
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Hook))]
        public int HookId { get; set; }
        public virtual Hook? Hook { get; set; }

        [ForeignKey(nameof(AlertTrigger))]
        public long? AlertTriggerId { get; set; }
        public virtual AlertTrigger? AlertTrigger { get; set; }

        [Required]
        public string Payload { get; set; }

        public int? StatusCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public int Attempts { get; set; } = 0;

        public DateTime? LastAttemptAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}