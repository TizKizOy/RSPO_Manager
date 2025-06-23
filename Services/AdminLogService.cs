using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.Linq;

namespace MyKpiyapProject.Services
{
    public class AdminLogService
    {
        private readonly AppDbContext _context;

        public AdminLogService()
        {
            _context = new AppDbContext();
        }

        public async Task AddAdminLogAsync(tbAdminLog logEntry)
        {
            using (var context = new AppDbContext())
            {
                context.AdminLogs.Add(logEntry);
                await context.SaveChangesAsync();
            }
        }
        public IQueryable<tbAdminLog> GetAllAdminLogsIQueryable()
        {
            return _context.AdminLogs; 
        }
        public List<tbAdminLog> GetAllAdminLogsAsync()
        {
            using (var context = new AppDbContext())
            {
                return context.AdminLogs.ToList();
            }
        }

        public void AddAdminLog(tbAdminLog logEntry)
        {
            using (var context = new AppDbContext())
            {
                context.AdminLogs.Add(logEntry);
            }
        }

        public List<tbAdminLog> GetAllAdminLogs()
        {
            using (var context = new AppDbContext())
            {
                return context.AdminLogs.ToList();
            }
        }

        // Read
        public tbAdminLog GetAdminLogById(int id)
        {
            return _context.AdminLogs.FirstOrDefault(a => a.LogID == id);
        }


        // Update
        public void UpdateAdminLog(tbAdminLog adminLog)
        {
            _context.Entry(adminLog).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        // Delete
        public void DeleteAdminLog(int id)
        {
            var adminLog = _context.AdminLogs.FirstOrDefault(a => a.LogID == id);
            if (adminLog != null)
            {
                _context.AdminLogs.Remove(adminLog);
                _context.SaveChanges();
            }
        }
    }
}
