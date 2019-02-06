using System;
using System.Collections.Generic;
using System.Text;
using ScheduleGenerator;

namespace ScheduleGenerater
{
    class Tester
    {
        static void Main(string[] args)
        {
            List<DateTime> nextVisit = ScheduleAlgorithm.start();
            ScheduleAlgorithm.manualOverride(nextVisit);

        }
    }
}
