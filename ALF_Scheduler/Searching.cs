using System;
using System.Linq;
using System.Collections.Generic;
using Facility;
using ALF_Scheduler.Utilities;


namespace ALF_Scheduler
{
    public class Searching
    {
        //need to get Db connection stuff from Kenny more than likely. Might be able to set it up like InspectionServices.cs\

        public static string searchFacilityName(string FacililtyName, List<Facility> FacilityList)
        {
            try
            {
                string fName = FacilityList.Single(f => f == FacilityName);//not sure if correct LINQ syntax here. Need to check with Kenny on DB connection stuff.
                return fName;
            }
            catch(Exception e)
            {
                errorLog(e);
            }
        }

        public static void errorLog(Exception e)
        {
            //send to ALF_Scheduler.Utilities.ErrorLogger
        }
    }
}