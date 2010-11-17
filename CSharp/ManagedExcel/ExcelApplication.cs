//Original source codes: http://www.codeproject.com/KB/COM/safecomwrapper.aspx
//Ported to Excel by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Collections;

namespace ManagedExcel
{	
	/// <summary>
	/// Wrapper for Excel.Application
    /// http://www.codeproject.com/KB/COM/safecomwrapper.aspx
	/// </summary>
	public class ExcelApplication
	{
		public static Application Create()
		{
			return (Application)COMWrapper.CreateInstance(typeof(Application));
		}
	}
}
