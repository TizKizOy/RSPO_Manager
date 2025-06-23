using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbPositionAndRole 
    {
        [Key]
        public int PositionAndRoleID { get; set; }

        [Required]
        public string PositionAndRoleName { get; set; }

        public virtual ICollection<tbEmployee> Employees { get; set; }
    }
}
