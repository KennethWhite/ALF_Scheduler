using System;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Import_Export
{

    /// <summary>
    /// This class is used to open and close references to Excel files
    /// </summary>
    public class ExcelImporterExporter
    {
        /// <summary>
        /// This method will attempt to open the Excel.Workbook located at the path specified
        /// and assign it into the out parameter xlWorkbook.
        /// </summary>
        /// <param name="filePath">Filepath to the Excel file</param>
        /// <param name="xlWorkbook">out parameter representing the Excel.Workbook</param>
        /// <returns>True upon success opening file, false on failure</returns>
        /// <exception>Throws NullReferenceException if Microsoft Excel is not installed on the systsem
        /// or if it cannot be opened</exception>
        public static bool LoadExcelFromFile(string filePath, out Excel.Application xlApp ,out Excel.Workbook xlWorkbook)
        {
            try
            {
                xlApp = new Excel.Application();

                if (xlApp == null)
                {
                    throw new NullReferenceException(
                        $"The program failed to load the file {filePath} because Microsoft Excel could not be opened.");
                }
                xlApp.DisplayAlerts = false;
                xlWorkbook = xlApp.Workbooks.Open($@"{filePath}");
                return true;
            }
            catch (NullReferenceException ex)
            {
                //This exception cannt be handled through our application
                //It will require the user installs Excel 2013 or later
                throw ex;
            }
            catch (Exception ex)
            {
                //TODO log
                xlWorkbook = null;
                xlApp = null;
                return false;
            }
        }

        /// <summary>
        /// This method will attempt to save the Excel.Workbook paramater back into
        /// the file it was originally loaded from
        /// </summary>
        /// <param name="workbook">The workbook to be saved</param>
        /// <returns>True upon success, False upon failure</returns>
        public static bool SaveWorkbookToOriginalFile(Excel.Workbook workbook)
        {
            try
            {
                workbook.Save();
                return true;
            }
            catch(Exception ex)
            {
                //TODO log
                return false;
            }

        }

        /// <summary>
        /// This method will attempt to save the <c>Excel.Workbook</c> parameter into
        /// the file path specified
        /// </summary>
        /// <param name="path">File path the workbook will be saved to</param>
        /// <param name="workbook">Workbook to be saved</param>
        /// <returns>True upon success, False upon failure</returns>
        public static bool SaveWorkbookToSpecifiedFile(string path, Excel.Workbook workbook)
        {
            try
            {
                workbook.SaveAs(path,default, default, default, true);
                return true;
            }
            catch (Exception ex)
            {
                //TODO log
                return false;
            }
        }

        /// <summary>
        /// This method will close the Excel application, its workbooks, and any direct references to a workbooks.
        /// </summary>
        /// <remarks>Note that any references to an Excel.Workbook must be closed separately from the application.</remarks>
        /// <param name="xlApp">The application reference</param>
        /// <param name="xlWorkBook">Reference to the Excel.Workbook to close</param>
        public static void CloseExcelApp(Excel.Application xlApp, Excel.Workbook xlWorkBook)
        {
            xlWorkBook.Close();
            xlApp.Workbooks.Close();
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp.Workbooks);
            Marshal.ReleaseComObject(xlApp);
        }


    }
}
