using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Models
{
    [MetadataType(typeof(TaskExtended))]
    public partial class Task
    {

        internal class TaskExtended
        {

            [Required(ErrorMessage = "*Task Date is Required")]
            public Nullable<System.DateTime> TaskDate { get; set; }

            [Required(ErrorMessage = "*Task Name is Required")]
            public string TaskName { get; set; }

            [Required(ErrorMessage = "*Task Description is Required")]
            public string TaskDescription { get; set; }
        } 
    }
}
