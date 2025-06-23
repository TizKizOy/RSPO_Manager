using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbGender
    {
        [Key]
        public int GenderID { get; set; }

        [Required]
        public string GenderName { get; set; }

        public virtual ICollection<tbEmployee> Employees { get; set; }
    }
}
