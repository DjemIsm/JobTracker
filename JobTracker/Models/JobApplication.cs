using System.ComponentModel.DataAnnotations;

namespace JobTracker.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Firmenname")]
        public required string CompanyName { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Jobtitel")]
        public required string JobTitle { get; set; }

        [Display(Name = "Bewerbungsdatum")]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        [StringLength(2000)]
        [Display(Name = "Notizen")]
        public string? Notes { get; set; }
    }
}
