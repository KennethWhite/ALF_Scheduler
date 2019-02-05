using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XML_Utils;

namespace ALF_Scheduler.Models
{
    public class Code : Entity
    {
        public string name { get; set; }
        public string description { get; set; }
        public int minMonth { get; set; }
        public int maxMonth { get; set; }

        public Code[] getCodesFromDefault()
        {
            XML_Utils.XML_Utils.
            return null;
        }
    }
}
