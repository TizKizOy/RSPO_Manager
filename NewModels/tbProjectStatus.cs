using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbProjectStatus
    {
        [Key]
        public int ProjectStatusID { get; set; }

        [Required]
        public string StatusName { get; set; }

        public virtual ICollection<tbProject> Projects { get; set; }
    }
}
