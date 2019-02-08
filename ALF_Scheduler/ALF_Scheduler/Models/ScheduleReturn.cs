using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler.Models
{
    //Represent the output of the GenerateSchedule method. Contains the average attained from that schedule, and a dictionary of the Facilities and their proposed next inspection
    public class ScheduleReturn
    {
        public IDictionary<Facility, DateTime> FacilitySchedule { get; set; }

        public double GlobalAvg { get; set; }
    }
}
