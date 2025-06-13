using MyKpiyapProject.NewModels;
using System;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class LoggingService
    {
        private readonly AdminLogService _adminLogService;

        public LoggingService()
        {
            _adminLogService = new AdminLogService();
        }

        public virtual async Task LogAction(int employeeId, string action, string eventType, string status)
        {
            var logEntry = new tbAdminLog
            {
                EmployeeID = employeeId,
                Action = action,
                EventType = eventType,
                DateTime = DateTime.Now,
                Status = status
            };

            await _adminLogService.AddAdminLogAsync(logEntry);
        }
    }
}
