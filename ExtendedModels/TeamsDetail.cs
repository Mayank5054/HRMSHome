using HRMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    [MetadataType(typeof(TeamsDetailsExtended))]
    public partial  class TeamsDetail
    {

        internal class TeamsDetailsExtended
        {
           
            [Required(ErrorMessage = "Assigned Role Can Not Be Empty")]
            public string AssignedRole { get; set; }

      
        }
    }
}