//Original source codes: http://www.codeproject.com/KB/COM/safecomwrapper.aspx
//Ported to Excel by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections;

namespace ManagedExcel
{
    /// <summary>
    /// Common properties that has appreared in almost all objects
    /// </summary>
    public interface Common : IDisposable {
        Application Application { get; }
        object Parent { get; }
    }
    public interface Collection : Common, IEnumerable {
        int Count { get; }
        void Remove(int index);
    }

/////////////////////////////////////////////////////////////////////////////
// You can add additional required classes and methods.
// To view full list of available classes and methods 
// view metadata of Microsoft.Office.Interop.Excel assembly 
// in Object Browser

    [ComProgId("Excel.Application")]
	public interface Application : Common {
        bool Visible { get; set; }
        String Version { get; }

		Workbook ActiveWorkbook();
		Workbooks Workbooks{ get; }

        Worksheet ActiveSheet { get; }
	
		void Quit();
	}

	public interface Workbooks : Collection {
		Workbook this[ int index ] { get; }
		Workbook Add(object Template);
        Workbook Open(String Filename, Object UpdateLinks, Object ReadOnly
            , Object Format, Object Password
            , Object WriteResPassword, Object IgnoreReadOnlyRecommended
            , Object Origin, Object Delimiter, Object Editable
            , Object Notify, Object Converter, Object AddToMru
            , Object Local, Object CorruptLoad);
        Workbook OpenXML(String Filename, Object Stylesheets, Object LoadOptions);
        Workbook Open(String Filename);
        void Close();
    }

	public interface Workbook : Common {
		Worksheets Sheets { get; }		
        String Title { get; set; }
        bool Saved { get; set; }

        void Save();
        void SaveAs(String Filename, Object FileFormat, Object Password
            , Object WriteResPassword, Object ReadOnlyRecommended
            , Object CreateBackup, XlSaveAsAccessMode AccessMode
            , Object ConflictResolution, Object AddToMru
            , Object TextCodepage, Object TextVisualLayout);
        void SaveAs(String Filename);

        void Close();
    }

	public interface Worksheets : Collection {
		Worksheet this[int index] { get; }
		Worksheet Add(object Template);
	}

	public interface Worksheet : Common {
        void Close();
        void Delete();
        void Activate();

        Range Cells { get; }
        Range Columns { get; }
        Range Rows { get; }
        Range UsedRange { get; }
		
        string Name { get; set; }
        int Index { get; }
	}


	public interface Range : Common {
        Range this[Object rowIndex, Object columnIndex] { get; set; }
        int Row { get; }
        int Column { get; }
        Range Columns { get; }
        Range Rows { get; }
        String Text { get; }

        object Value { get; set;  }
        object Value2 { get; set; }
	}

    public enum XlSaveAsAccessMode {
        //default (don't change the access mode)
        xlNoChange = 1,
        //(share list)
        xlShared = 2,
        //(exclusive mode)
        xlExclusive = 3
    }
}
