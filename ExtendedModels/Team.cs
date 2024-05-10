using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    [MetadataType(typeof(TeamExtended))]
    public partial class Team
    { 
        internal class TeamExtended
        {

            [Required(ErrorMessage ="Please Select Team Leader")]
            public Nullable<int> TeamLeader { get; set; }
            [Required(ErrorMessage = "Please Select Team Name")]
            public string TeamName { get; set; }

        }
    }
}