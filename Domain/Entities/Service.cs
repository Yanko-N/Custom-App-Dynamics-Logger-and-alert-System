using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Environment { get; set; }

        [StringLength(30)]
        public string Version { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<CustomLog> Logs { get; set; }
        public virtual ICollection<Hook> Hooks { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
    }
}