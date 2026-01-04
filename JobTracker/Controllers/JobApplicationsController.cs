using JobTracker.Data;
using JobTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace JobTracker.Controllers
{
    [Authorize]
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q, ApplicationStatus? status)
        {
            IQueryable<JobApplication> query = _context.JobApplications;

            query = ApplyFilter(query, q, status);

            var jobApplications = await query
                .OrderByDescending(x => x.ApplicationDate)
                .ToListAsync();

            // Für "keep values" im View
            ViewBag.Q = q;
            ViewBag.Status = status;

            return View(jobApplications);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplication jobApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job application created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(jobApplication);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var jobapplication = await _context.JobApplications.FindAsync(id);        
            if (jobapplication == null)
            {
                return NotFound();
            }
            return View(jobapplication);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            return View(jobApplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("Id,CompanyName,JobTitle,ApplicationDate,Status,Notes")] JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(jobApplication);
            }
            _context.Update(jobApplication);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job application updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete") ]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
                return NotFound();
            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Job application deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? q, ApplicationStatus? status)
        {
            IQueryable<JobApplication> query = _context.JobApplications;

            query = ApplyFilter(query, q, status);

            var apps = await query
                .OrderByDescending(x => x.ApplicationDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            var sb = new StringBuilder();

            sb.AppendLine("Id,CompanyName,JobTitle,ApplicationDate,Status,Notes");

            foreach (var app in apps)
            {
                var line = string.Join(",",
                    EscapeCsv(app.Id.ToString()),
                    EscapeCsv(app.CompanyName),
                    EscapeCsv(app.JobTitle),
                    EscapeCsv(app.ApplicationDate.ToString("yyyy-MM-dd")),
                    EscapeCsv(app.Status.ToString()),
                    EscapeCsv(app.Notes ?? "")
                    );
                sb.AppendLine(line);
            }

            var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            var bytes = utf8WithBom.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "JobApplications.csv");
        }

        private static string EscapeCsv(string v)
        {
            if (v.IndexOfAny([',', '"', '\r', '\n']) >= 0)
            {
                return $"\"{v.Replace("\"", "\"\"")}\"";
            }
            return v;
        }

        private static IQueryable<JobApplication> ApplyFilter(
            IQueryable<JobApplication> query, string? q, ApplicationStatus? status)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x =>
                    x.CompanyName.Contains(term) ||
                    x.JobTitle.Contains(term));
            }
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            return query;
        }
    }
}
