using JobTracker.Data;
using JobTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        

        public async Task<IActionResult> Index()
        {
            var groupedData = await _context.JobApplications
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var statusCounts = groupedData.ToDictionary(x => x.Status, x => x.Count);

            foreach (ApplicationStatus status in Enum.GetValues(typeof(ApplicationStatus)))
            {
                if (!statusCounts.ContainsKey(status))
                {
                    statusCounts[status] = 0;
                }
            }

            // Kürzlich hinzugefügte Bewerbungen (z.B. die letzten 5)
            var recentApplications = await _context.JobApplications
                .OrderByDescending(x => x.ApplicationDate)
                .ThenByDescending(x => x.Id)
                .Take(5)
                .ToListAsync();

            var dashboardViewModel = new DashboardViewModel
            {
                ApplicationStatusCounts = statusCounts,
                RecentApplications = recentApplications
            };

            return View(dashboardViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
