using MyKpiyapProject.NewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKpiyapProject.Events
{
    public class ProjectEventArgs : EventArgs
    {
        public tbProject Projects { get; }

        public ProjectEventArgs(tbProject projects)
        {
            Projects = projects;
        }
    }
}
