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
        public class CheckDateAfterToday : ValidationAttribute {

            public override bool IsValid(object value)
            {
                DateTime date = (DateTime)value;
                return date.Date <= DateTime.Today;
            }
        }

        internal class TaskExtended
        {

            [Required(ErrorMessage = "*Task Date is Required")]
            //[CheckDateAfterToday(ErrorMessage ="Date Must Be Before Current Date")]
            public Nullable<System.DateTime> TaskDate { get; set; }

            [Required(ErrorMessage = "*Task Name is Required")]
            public string TaskName { get; set; }

            [Required(ErrorMessage = "*Task Description is Required")]
            public string TaskDescription { get; set; }
        } 
    }
}
