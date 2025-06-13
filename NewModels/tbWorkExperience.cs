using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbWorkExperience
    {
        [Key]
        public int ExperienceID { get; set; }

        public string ExperienceName { get; set; }

        public virtual ICollection<tbEmployee> Employee { get; set; }
    }
}
