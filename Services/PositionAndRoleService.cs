using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Services
{
    public class PositionAndRoleService
    {
        private readonly AppDbContext _context;

        public PositionAndRoleService()
        {
            _context = new AppDbContext();
        }

        public int GetOrCreatePositionAndRoleId(string positionAndRoleName)
        {
            var positionAndRole = _context.PositionAndRoles.FirstOrDefault(pr => pr.PositionAndRoleName == positionAndRoleName);

            if (positionAndRole == null)
            {
                positionAndRole = new tbPositionAndRole { PositionAndRoleName = positionAndRoleName };
                _context.PositionAndRoles.Add(positionAndRole);
                _context.SaveChanges();
            }

            return positionAndRole.PositionAndRoleID;
        }

        public tbPositionAndRole GetPositionAndRoleById(int positionAndRoleId)
        {
            return _context.PositionAndRoles.Find(positionAndRoleId);
        }
    }
}
