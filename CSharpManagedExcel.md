There is a nice article on CodeProject - <a href='http://www.codeproject.com/KB/COM/safecomwrapper.aspx'>"SafeCOMWrapper - Managed Disposable Strongly Typed safe wrapper to late bound COM"</a>. It describes elegance approach to work with Microsoft.Outlook from C# using late bound COM. I have used this approach to work with Microsoft Excel from C# in the same way. It works perfect for me, thank a lot to the author of the original article.

Sample of code (how to work with Excel from C# using late bound COM):
```
using (Application app = ExcelApplication.Create()) {
    String s = app.Version;
    app.Visible = true; // make excel visible
    using (Workbook wb = app.Workbooks.Add(Type.Missing)) {
        wb.Title = "new workbook";
        using (Worksheets worksheets = wb.Sheets) {
            using (Worksheet ws = worksheets[1]) {
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
        wb.Saved = true;
        wb.Close();
    }
    app.Quit();
}
```


---

<a href='http://derevyanko.blogspot.com/2009/05/excel-c.html'>Работа с Excel из C#</a>

<a href='http://www.codeproject.com/KB/COM/safecomwrapper.aspx'>SafeCOMWrapper - Managed Disposable Strongly Typed safe wrapper to late bound COM</a> - original article on code project.

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=ManagedExcel.7z&can=2&q='>Download source codes of Managed Excel project</a>

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/CSharp/ManagedExcel'>View source codes of Managed Excel project</a>