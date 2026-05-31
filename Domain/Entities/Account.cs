using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<ApiKey> ApiKeys { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
