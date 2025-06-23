using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyKpiyapProject.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService()
        {
            _context = new AppDbContext();
        }

        // Create
        public void AddReport(tbReport report)
        {
            _context.Reports.Add(report);
            _context.SaveChanges();
        }

        // Read
        public tbReport GetReportById(int id)
        {
            return _context.Reports.FirstOrDefault(r => r.ReportID == id);
        }

        public List<tbReport> GetAllReports()
        {
            // Очищаем кэш контекста
            _context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Detached);

            return _context.Reports
                .AsNoTracking()
                .ToList();
        }

        // Update
        public void UpdateReport(tbReport report)
        {
            var existingReport = _context.Employees.Find(report.ReportID);

            if (existingReport != null)
            {
                _context.Entry(existingReport).CurrentValues.SetValues(report);
            }
            else
            {
                _context.Reports.Attach(report);
                _context.Entry(report).State = System.Data.Entity.EntityState.Modified;
            }

            _context.SaveChanges();
        }

        // Delete
        public void DeleteReport(int? id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.ReportID == id);
            if (report != null)
            {
                _context.Reports.Remove(report);
                _context.SaveChanges();
            }
        }
    }
}
