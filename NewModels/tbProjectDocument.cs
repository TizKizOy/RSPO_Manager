using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.NewModels
{
    public class tbProjectDocument
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public byte[] DocxData { get; set; }

        public virtual ICollection<tbProject> Projects { get; set; }
    }
}
