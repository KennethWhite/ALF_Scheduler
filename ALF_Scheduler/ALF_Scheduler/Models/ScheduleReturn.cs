using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler.Models
{
    public struct ScheduleReturn
    {
        public IDictionary<Facility, DateTime> FacilitySchedule { get; set; }

        public double GlobalAvg { get; set; }

        public override string ToString()
        {
            return "Global Average: " + GlobalAvg;
        }
    }
}
