namespace JobTracker.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public required string CompanyName { get; set; }
        public required string JobTitle { get; set; }
        public DateTime ApplicationDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
