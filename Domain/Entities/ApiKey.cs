using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }

        [Required]
        [StringLength(64)]
        public string KeyHash { get; set; }

        [StringLength(100)]
        public string Label { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        public DateTime? LastUsedAt { get; set; }
    }
}