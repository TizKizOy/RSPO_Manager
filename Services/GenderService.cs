using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class GenderService
    {
        private readonly AppDbContext _context;

        public GenderService()
        {
            _context = new AppDbContext();
        }

        public int GetOrCreateGenderId(string genderName)
        {
            var gender = _context.Genders.FirstOrDefault(g => g.GenderName == genderName);

            if (gender == null)
            {
                gender = new tbGender { GenderName = genderName };
                _context.Genders.Add(gender);
                _context.SaveChanges();
            }

            return gender.GenderID;
        }

        public tbGender GetGenderById(int genderId)
        {
            return _context.Genders.Find(genderId);
        }
    }
}
