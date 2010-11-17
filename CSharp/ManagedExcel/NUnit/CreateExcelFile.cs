//Original source codes: http://www.codeproject.com/KB/COM/safecomwrapper.aspx
//Ported to Excel by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ManagedExcel.NUnit {
    /// <summary>
    /// To execute these tests use NUnit 2.5
    /// http://www.nunit.org/index.php
    /// </summary>
    [TestFixture]
    class CreateExcelFile {

        /// <summary>
        /// Test: open existed excel file "test.xls" and read some values from it
        /// </summary>
        [Test]
        public void OpenFile() {
            using (Application app = ExcelApplication.Create()) {
                String s = app.Version;
                using (Workbook wb = app.Workbooks.Open(@"test.xls", Type.Missing
                     , Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                     , Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                     , Type.Missing, Type.Missing, Type.Missing)) 
                {
                    String title = wb.Title;
                    using (Worksheet ws = wb.Sheets[1]) {
                        using (Range r11 = ws.Cells[1, 1]) { //try to read first cell
                            String svalue11 = ws.Cells[1, 1].Value.ToString();
                        }
                    }

                    wb.Close();
                }
                app.Quit();
            }
        }

        /// <summary>
        /// Test: create new excel file and put some numbers to it
        /// </summary>
        [Test]
        public void CreateSimpleFile() {
            using (Application app = ExcelApplication.Create()) {
                String s = app.Version;
                app.Visible = true; // make excel visible
                using (Workbook wb = app.Workbooks.Add(Type.Missing)) {
                    wb.Title = "new workbook";
                    using (Worksheets worksheets = wb.Sheets) {
                        int count_sheets = worksheets.Count; //try to get count of sheets
                        using (Worksheet ws = wb.Sheets[1]) {
                            Range usedrange = ws.UsedRange; //try to get used range

                            //try to assign some values to some cells
                            using (Range cells = ws.Cells) {
                                for (int i = 1; i < 10; ++i) {
                                    using (Range r = cells[i, i]) {
                                        r.Value = i * i;
                                    }
                                }
                            }

                        }
                    }
                    // wb.Saved = true;
                    // wb.Close();
                }
                // app.Quit();
            }
        }
    }
}
