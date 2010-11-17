//Original source codes: http://www.codeproject.com/KB/COM/safecomwrapper.aspx
//Ported to Excel by Victor Derevyanko, dvpublic0@gmail.com, http://code.google.com/p/dvsrc/
//{$Id$}

using System;
using System.Diagnostics;
using System.Runtime.Remoting.Proxies;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;
using System.Reflection;

namespace ManagedExcel
{
    /// <summary>
    /// http://www.codeproject.com/KB/COM/safecomwrapper.aspx
    /// </summary>
	public class DisposableCOMProxy : RealProxy
	{
		public object COM;

		/// <summary>
		/// We will be using late bound COM operations. The COM object
		/// is created from the Prog ID instead of CLSID which makes it
		/// a version independent approach to instantiate COM.
		/// </summary>
		/// <param name="progID">Prog ID e.g. Outlook.Application</param>
		/// <returns></returns>
		private static object CreateCOM( string progID )
		{
			// Instantiate the COM object using late bound
			Type comType = Type.GetTypeFromProgID( progID, true );
			return Activator.CreateInstance( comType );
		}

		public static IDisposable Create( string progID, Type interfaceType )
		{	
			object theCOM = CreateCOM( progID );
			DisposableCOMProxy wrapper = new DisposableCOMProxy( theCOM, interfaceType );
			return wrapper.GetTransparentProxy() as IDisposable;
		}

		public DisposableCOMProxy( object theCOM, Type interfaceType ) :base( interfaceType )
		{
			this.COM = theCOM;
		}

		public override IMessage Invoke(IMessage msg)
		{
			IMethodCallMessage callMessage = msg as IMethodCallMessage;

			object returnValue = null;

			MethodInfo method = callMessage.MethodBase as MethodInfo;					
			
			// We intercept all method calls on the interface and delegate the method
			// call to the COM reference.
			// Only exception is for "Dispose" which needs to be called on this class
			// in order to release the COM reference. COM reference does not have dispose
			if( method.Name == "Dispose" )
			{
				this.Release();
			}
			else
			{

				object invokeObject = this.COM;
				Type invokeType = this.COM.GetType();

				// Get Property called: Retrieve property value
				if( method.Name.StartsWith("get_") )
				{
					string propertyName = method.Name.Substring(4);
					returnValue = invokeType.InvokeMember( propertyName, BindingFlags.GetProperty, null,
						invokeObject, callMessage.InArgs );
				}
					// Set Property Called: Set the property value
				else if( method.Name.StartsWith("set_") )
				{
					string propertyName = method.Name.Substring(4);						
					returnValue = invokeType.InvokeMember( propertyName, BindingFlags.SetProperty, null,
						invokeObject, callMessage.InArgs );
				}
					// Regular method call
				else
				{
					returnValue = invokeType.InvokeMember( method.Name, BindingFlags.InvokeMethod, null,
						invokeObject, callMessage.Args );
				}

				// Now check if the method return value is also an interface. if it is an 
				// interface, then we are interested to intercept that too
				if( method.ReturnType.IsInterface && null != returnValue )
				{	
					// Return a intercepting wrapper for the com object
					DisposableCOMProxy proxy = new DisposableCOMProxy( returnValue, method.ReturnType );
					returnValue = proxy.GetTransparentProxy();
				}
				else if( method.ReturnType.IsEnum && null != returnValue )
				{
					// Convert to proper enum type
					returnValue = Enum.Parse(method.ReturnType, returnValue.ToString());
				}
			}

			// Construct a return message which contains the return value 
			ReturnMessage returnMessage = new ReturnMessage( returnValue, null, 
				0, callMessage.LogicalCallContext,
				callMessage );

			return returnMessage;
		}

		/// <summary>
		/// Safely release the COM object
		/// </summary>
		private void Release()
		{
			if( null == this.COM ) return;

			while( Marshal.ReleaseComObject( this.COM ) > 0 );
			this.COM = null;

			Debug.WriteLine( "COM released successfully" );
		}

		~DisposableCOMProxy()
		{
			this.Release();
		}		
	}

}
