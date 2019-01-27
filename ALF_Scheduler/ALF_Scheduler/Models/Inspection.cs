using ALF_Scheduler.Models;
using System;

namespace ALF_Scheduler.Domain.Models
{
    public class Inspection : Entity
    {
        public DateTime InspectionDate { get; set; }
        public string Licensor { get; set; }
        public string Comments { get; set; }
    }
}