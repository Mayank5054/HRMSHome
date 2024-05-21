using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class JWTModel
    {
        public string Email { get; set; }
    
        public int RoleId { get; set; }
        public int userId { get; set; }
    }
}