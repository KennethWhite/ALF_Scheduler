using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Excel = Microsoft.Office.Interop.Excel;
using Excel_Import_Export;

namespace Scheduler
{
    [TestClass]
    public class ExcelImporterExporterTests
    {
        private Excel.Workbook ExcelWorkBook { get; set; }

        [ClassInitialize]
        public static void CreateExcelFileForTests(TestContext textContext)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook WorkBook = xlApp.Workbooks.Add(Type.Missing);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)WorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Name";
            xlWorkSheet.Cells[2, 1] = "1";
            xlWorkSheet.Cells[2, 2] = "One";
            xlWorkSheet.Cells[3, 1] = "2";
            xlWorkSheet.Cells[3, 2] = "Two";
            WorkBook.SaveAs(@"/TestData/TestExcelLoad.xlsx");
        }

        [TestMethod]
        public void LoadExcelFromFile_Success()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(Type.Missing);
            bool result = ExcelImporterExporter.LoadExcelFromFile(
                @"/TestData/TestExcelLoad.xlsx", out xlWorkBook);
            Assert.IsTrue(result);
        }


        [ClassCleanup]
        public static void CleanupExcelFileForTests()
        {
            System.IO.File.Delete(@"/TestData/TestExcelLoad.xlsx");
        }

    }
}
