using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class ChatExtended
    {
        public List<Chat> listOfSeenChats {  get; set; }
        public List<Chat> listOfUsSeenChats { get; set; }  
    }
}