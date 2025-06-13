using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class ProjectStatusService
    {
        private readonly AppDbContext _context;

        public ProjectStatusService()
        {
            _context = new AppDbContext();
        }

        public int GetProjectStatusIdByName(string statusName)
        {
            var status = _context.ProjectStatuses.FirstOrDefault(s => s.StatusName == statusName);
            return status?.ProjectStatusID ?? 1; // Возвращаем ID по умолчанию, если статус не найден
        }

        public string GetProjectStatusNameById(int statusId)
        {
            var status = _context.ProjectStatuses.FirstOrDefault(s => s.ProjectStatusID == statusId);
            return status?.StatusName ?? "Unknown"; // Возвращаем имя по умолчанию, если статус не найден
        }

        public tbProjectStatus GetProjectStatusById(int statusId)
        {
            return _context.ProjectStatuses.FirstOrDefault(s => s.ProjectStatusID == statusId);
        }
    }
}
