using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MyKpiyapProject.Services
{
    public class ProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService()
        {
            _context = new AppDbContext();
        }

        // Create
        public void AddProject(tbProject project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        // Read
        public tbProject GetProjectById(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.ProjectID == id);
        }

        public List<tbProject> GetAllProjects()
        {
            return _context.Projects.AsNoTracking().ToList();
        }

        // Update
        public virtual void UpdateProject(tbProject project)
        {
            // Поиск существующего проекта по ID
            var existingProject = _context.Projects.Find(project.ProjectID);

            if (existingProject != null)
            {
                // Если проект найден, обновляем его данные
                _context.Entry(existingProject).CurrentValues.SetValues(project);
            }
            else
            {
                // Если проект не найден, присоединяем переданный объект проекта к контексту
                // и помечаем его как измененный
                _context.Projects.Attach(project);
                _context.Entry(project).State = System.Data.Entity.EntityState.Modified;
            }

            // Сохраняем изменения в базе данных
            _context.SaveChanges();
        }



        // Delete
        public void DeleteProject(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectID == id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
        }
    }
}
