using System;
using System.IO;
using Excel_Import_Export;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scheduler
{
    /* This test class is far too slow as it uses significant amounts of file IO.
     * Slow tests discourage running the test suite and should be avoided.
     * It will be worth my time to work on an alternative 
     */

    [TestClass]
    public class ExcelImporterExporterTests
    {
        private Workbook ExcelWorkBook { get; set; }

        [ClassInitialize]
        public static void CreateExcelFileForTests(TestContext textContext)
        {
            InitializeTestDirectory();

            var xlApp = new Application();
            var WorkBook = xlApp.Workbooks.Add(Type.Missing);
            var xlWorkSheet = (Worksheet) WorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Name";
            xlWorkSheet.Cells[2, 1] = "1";
            xlWorkSheet.Cells[2, 2] = "One";
            xlWorkSheet.Cells[3, 1] = "2";
            xlWorkSheet.Cells[3, 2] = "Two";
            xlApp.DisplayAlerts = false;
            WorkBook.SaveAs(Environment.CurrentDirectory + @"\TestData\TestALFScheduler",
                XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            ExcelImporterExporter.CloseExcelApp(xlApp, WorkBook);
        }

        private static void InitializeTestDirectory()
        {
            var destinationDirectory = Environment.CurrentDirectory + @"\TestData";

            var directory = new DirectoryInfo(destinationDirectory);
            if (!directory.Exists) Directory.CreateDirectory(destinationDirectory);
            //FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
            //DirectorySecurity ds = directory.GetAccessControl();
            //ds.AddAccessRule(fsar);
            //directory.SetAccessControl(ds);
        }


        [TestMethod]
        public void LoadExcelFromFile_Success()
        {
            Application xlApp;
            Workbook xlWorkBook;
            var result = ExcelImporterExporter.LoadExcelFromFile(
                Environment.CurrentDirectory + @"\TestData\TestALFScheduler", out xlApp, out xlWorkBook);
            if (!result) Assert.Fail();
            Assert.IsTrue(result);
            ExcelImporterExporter.CloseExcelApp(xlApp, xlWorkBook);
        }

        [TestMethod]
        public void LoadExcelFromFile_VerifyColumns_Success()
        {
            Application xlApp;
            Workbook xlWorkBook;
            var result = ExcelImporterExporter.LoadExcelFromFile(
                Environment.CurrentDirectory + @"\TestData\TestALFScheduler", out xlApp, out xlWorkBook);
            if (!result) Assert.Fail();
            var xlWorkSheet = (Worksheet) xlWorkBook.Worksheets.get_Item(1);
            Assert.AreEqual("ID", xlWorkSheet.Cells[1, 1].Value.ToString());
            Assert.AreEqual("Name", xlWorkSheet.Cells[1, 2].Value.ToString());
            ExcelImporterExporter.CloseExcelApp(xlApp, xlWorkBook);
        }

        [TestMethod]
        public void SaveExcelToOriginalFile_ColumnsChanged()
        {
            Application xlApp;
            Workbook xlWorkBook;
            var result = ExcelImporterExporter.LoadExcelFromFile(
                Environment.CurrentDirectory + @"\TestData\TestALFScheduler", out xlApp, out xlWorkBook);
            if (!result) Assert.Fail();
            var xlWorkSheet = (Worksheet) xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Cells[1, 1] = "TEST";
            xlWorkSheet.Cells[1, 2] = "VALUES";
            result = ExcelImporterExporter.SaveWorkbookToOriginalFile(xlWorkBook);
            if (!result) Assert.Fail();
            ExcelImporterExporter.CloseExcelApp(xlApp, xlWorkBook);

            result = ExcelImporterExporter.LoadExcelFromFile(
                Environment.CurrentDirectory + @"\TestData\TestALFScheduler", out xlApp, out xlWorkBook);
            if (!result) Assert.Fail();
            xlWorkSheet = (Worksheet) xlWorkBook.Worksheets.get_Item(1);
            Assert.AreEqual("TEST", xlWorkSheet.Cells[1, 1].Value.ToString());
            Assert.AreEqual("VALUES", xlWorkSheet.Cells[1, 2].Value.ToString());
            ExcelImporterExporter.CloseExcelApp(xlApp, xlWorkBook);
        }

        [ClassCleanup]
        public static void CleanupExcelFileForTests()
        {
            File.Delete(Environment.CurrentDirectory + @"\TestData\TestALFScheduler");
        }
    }
}