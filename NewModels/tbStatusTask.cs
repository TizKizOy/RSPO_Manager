using MyKpiyapProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbStatusTask
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        public string StatusName { get; set; }

        public virtual ICollection<tbTask> Tasks { get; set; }

    }
}
