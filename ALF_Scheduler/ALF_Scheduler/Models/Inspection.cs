using System;
using ALF_Scheduler.Models;

namespace ALF_Scheduler.Models
{
    public class Inspection : Entity
    {
        public int FacilityID { get; set; }
        public DateTime InspectionDate { get; set; }
        public string Licensor { get; set; }
        public string Comments { get; set; }
        public Code Code { get; set; }
        public static Inspection Inspection_Default { get; } = new Inspection();
    }
}