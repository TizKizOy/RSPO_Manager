using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class WorkExperienceServices
    {
        private readonly AppDbContext _context;
        public WorkExperienceServices()
        {
            _context = new AppDbContext();
        }

        public int GetOrCreateExperienceId(string ExpName)
        {
            var exp = _context.workExperiences.FirstOrDefault(g => g.ExperienceName == ExpName);

            if (exp == null)
            {
                exp = new tbWorkExperience { ExperienceName = ExpName };
                _context.workExperiences.Add(exp);
                _context.SaveChanges();
            }

            return exp.ExperienceID;
        }

        public tbWorkExperience GetExperienceById(int expId)
        {
            return _context.workExperiences.Find(expId);
        }
    }
}
