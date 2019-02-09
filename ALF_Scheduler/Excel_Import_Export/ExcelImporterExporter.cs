﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using ALF_Scheduler.Utilities;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel_Import_Export
{

    /// <summary>
    /// This class is used to open and close references to Excel files
    /// </summary>
    public static class ExcelImporterExporter
    {
        /// <summary>
        /// This method will attempt to open the Excel.Workbook located at the path specified
        /// and assign it into the out parameter xlWorkbook.
        /// </summary>
        /// <param name="filePath">Filepath to the Excel file</param>
        /// <param name="xlWorkbook">out parameter representing the Excel.Workbook</param>
        /// <param name="xlApp">out parameter representing the Excel.Application</param>
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
                ErrorLogger.LogInfo($"Failed to load Excel workbook from file {filePath}. This is likely" +
                                    $" caused because Microsoft Excel could not be opened or" +
                                    $" is not installed upon this machine.", ex);
                throw;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogInfo($"Failed to load Excel workbook from file {filePath}.", ex);
                xlWorkbook = null;
                xlApp = null;
                return false;
            }
        }

        /// <summary>
        /// This method will attempt to save the Excel.Workbook parameter back into
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
                ErrorLogger.LogInfo("Failed to save data into original file.", ex);
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
                ErrorLogger.LogInfo($"Failed to save Excel Workbook to file '{path}'specified.", ex);
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

        /// <summary>
        /// This method will delete the file at the path specified.
        /// </summary>
        /// <warning>This action is destructive and irreversible.</warning>
        /// <param name="path">The full path to the file to be deleted</param>
        public static void DeleteSpecifiedFile(string path)
        {
            if (!File.Exists(path))
            {
                ErrorLogger.LogInfo($"Failed to delete file:  {path}");
            }
            File.Delete(path);
        }
        
        

    }
}
