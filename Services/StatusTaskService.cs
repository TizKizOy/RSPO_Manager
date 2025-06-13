using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class StatusTaskService
    {
        private readonly AppDbContext _context;

        public StatusTaskService()
        {
            _context = new AppDbContext();
        }

        public int GetStatusIdByName(string statusName)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusName == statusName);
            return status?.StatusID ?? 1; // Возвращаем ID по умолчанию, если статус не найден
        }

        public string GetStatusNameById(int statusId)
        {
            var status = _context.Statuses.FirstOrDefault(s => s.StatusID == statusId);
            return status?.StatusName ?? "Unknown"; // Возвращаем имя по умолчанию, если статус не найден
        }

        public tbStatusTask GetStatusById(int statusId)
        {
            return _context.Statuses.FirstOrDefault(s => s.StatusID == statusId);
        }
    }
}
