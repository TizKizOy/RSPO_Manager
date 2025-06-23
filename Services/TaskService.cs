using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyKpiyapProject.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;

        public TaskService()
        {
            _context = new AppDbContext();
        }

        // Create
        public virtual void AddTask(tbTask task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        // Read
        public IEnumerable<tbTask> GetTasksByProject(int id)
        {
            return _context.Tasks.Where(t => t.ProjectID == id).ToList();
        }

        public tbTask GetTaskById(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.TaskID == id);
        }

        public virtual List<tbTask> GetAllTasks()
        {
            return _context.Tasks.AsNoTracking().ToList() ?? new List<tbTask>();
        }

        // Update
        public virtual void UpdateTask(tbTask task)
        {
            // Получаем текущую сущность из базы данных
            var existingTask = _context.Tasks.Find(task.TaskID);

            if (existingTask != null)
            {
                // Обновляем свойства существующей сущности
                _context.Entry(existingTask).CurrentValues.SetValues(task);
                _context.SaveChanges();
            }
            else
            {
                // Если сущность не существует, добавляем её в контекст и устанавливаем состояние в Modified
                _context.Tasks.Attach(task);
                _context.Entry(task).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }


        // Delete
        public virtual void DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TaskID == id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}
