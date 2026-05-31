using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CustomLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(10)]
        public string Level { get; set; }

        public Guid TraceId { get; set; }

        [Required]
        public string Message { get; set; }

        public string? StackTrace { get; set; }
    }
}