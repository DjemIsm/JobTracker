namespace JobTracker.Models
{
    public class DashboardViewModel
    {
        public Dictionary<ApplicationStatus, int> ApplicationStatusCounts { get; set; } = new();
        public List<JobApplication> RecentApplications { get; set; } = new();
    }
}
