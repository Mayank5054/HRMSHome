using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Models
{
    [MetadataType(typeof(EmployeeExtended))]
    public partial class Employee
    {

        internal class EmployeeExtended
        {
            [EmailAddress(ErrorMessage ="Email Address Is Not Valid")]
            [Required(ErrorMessage ="*Email Is Required")]
            public string Email { get; set; }
            [Required(ErrorMessage = "*Password Is Required")]
            [StringLength(12,MinimumLength =6,ErrorMessage ="Password Should be Greater Than 6 Characters")]
            public string Password { get; set; }

            [Required(ErrorMessage = "*First Name Is Required")]
            [RegularExpression(@"[^\s]+", ErrorMessage = "Spaces Are Not Allowed")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "*Last Name Is Required")]
            [RegularExpression(@"[^\s]+", ErrorMessage = "Spaces Are Not Allowed")]
            public string LastName { get; set; }
            [Required(ErrorMessage ="Birth Date Is Required")]
            public Nullable<System.DateTime> BirthDate { get; set; }
            [Required(ErrorMessage = "Gende Must Be Selected")]
            public string Gender { get; set; }
            [Required(ErrorMessage = "Designation Must  Be Selected")]
            public Nullable<int> DepartmentId { get; set; }
            [Required(ErrorMessage = "Reporting Person Must Be Selected")]
            public Nullable<int> ReportingPerson { get; set; }

         
        }
    }
}
