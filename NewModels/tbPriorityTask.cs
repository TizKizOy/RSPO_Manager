using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbPriorityTask
    {
        [Key]
        public int PriorityID { get; set; }

        [Required]
        public string PriorityName { get; set; }

        public virtual ICollection<tbTask> Tasks { get; set; }
    }
}
