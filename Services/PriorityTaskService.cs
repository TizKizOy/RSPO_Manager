using Microsoft.Office.Interop.Excel;
using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class PriorityTaskService
    {
        private readonly AppDbContext _context;

        public PriorityTaskService()
        {
            _context = new AppDbContext();
        }

        public int GetPriorityIdByName(string priorityName)
        {
            var priority = _context.Priorities.FirstOrDefault(p => p.PriorityName == priorityName);
            return priority?.PriorityID ?? 1; // Возвращаем ID по умолчанию, если приоритет не найден
        }

        public string GetPriorityNameById(int priorityId)
        {
            var priority = _context.Priorities.FirstOrDefault(p => p.PriorityID == priorityId);
            return priority?.PriorityName ?? "Unknown"; // Возвращаем имя по умолчанию, если приоритет не найден
        }

        public tbPriorityTask GetPriorityById(int priorityId)
        {
            return _context.Priorities.FirstOrDefault(p => p.PriorityID == priorityId);
        }
    }
}
