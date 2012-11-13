using System;
using UnityEngine;

namespace a2uTest {
	/// <summary>
	/// It tries to call Android plugin functions.
	/// The instance of Caller can be created in two ways:
	/// 1) create Caller as static class in MonoBehaviour.Start
	/// 2) create Caller as non-static class in MonoBehaviour.OnGUI
	/// There are several variants of calls:
	/// 1. call non-static functions of MyMainActivity
	/// 2. call static functions of MyMainActivity
	/// 3. call non-static function of MyClass
	/// 4. call static function of MyClass
	/// Additional variations:
	/// 5. make call from thread
	/// </summary>
	public class Caller {
		/// <summary>
		/// "Static" or "non static" for 1) and 2)
		/// </summary>
		private readonly String _InstanceName;
		
		private readonly AndroidJavaClass _ActivityClass;
		private readonly AndroidJavaObject _ActivityObject;	
		
		private readonly AndroidJavaClass _MyClassClass;
		private readonly AndroidJavaObject _MyClassObject;
		
		private readonly AndroidJavaClass _MyActivityClass;
		//private readonly AndroidJavaObject _MyActivityObject; //wrong way..
		
		
		public Caller(String instanceName) {
			_InstanceName = instanceName;
			
			to_log("Caller.Start");
				
			_MyActivityClass = new AndroidJavaClass("com.example.u2aTest.MyMainActivity"); 
			ensure_not_null(_MyActivityClass, "com.example.u2aTest.MyMainActivity class");
			to_log("Caller.1");			
						try {
							AndroidJavaObject my_activity_object = _MyActivityClass.GetStatic<AndroidJavaObject>("currentActivity");	//doesn't work				
							ensure_not_null(_ActivityClass, "com.example.u2aTest.MyMainActivity object");
							to_log("Caller.4");
						} catch (Exception ex) {
							to_log (ex.Message);
						}
			to_log("Caller.3");				
				
						
			_ActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			ensure_not_null(_ActivityClass, "current activity class");
			to_log("Caller.15");

			_ActivityObject = _ActivityClass.GetStatic<AndroidJavaObject>("currentActivity");					
			ensure_not_null(_ActivityClass, "current activity object");
			to_log("Caller.20");

			_MyClassClass = new AndroidJavaClass("com.example.u2aTest.MyClass");
			ensure_not_null(_MyClassClass, "my class class");
			to_log("Caller.30");
			
			_MyClassObject = new AndroidJavaObject("com.example.u2aTest.MyClass");
			ensure_not_null(_MyClassClass, "my class object");
			to_log("Caller.40");
					
			
			to_log("Caller.Finish");
		}
			
		public void makeTests() {
			to_log("makeTests.Start");
			make_non_static_calls_activity();
			to_log("makeTests.10");
			make_static_calls_activity();
			to_log("makeTests.20");
			make_non_static_calls_myclass();
			to_log("makeTests.30");
			make_static_calls_myclass();
			to_log("makeTests.40");
			make_non_static_calls_activity_int_params();
			to_log("makeTests.50");
			make_calls_from_thread();
			to_log("makeTests.End");
		}
		private void to_log(String message) {
			Debug.Log("Caller." + _InstanceName + ":" + message);
		}				
		private void ensure_not_null(object mustBeNotNull, String message) {
			if (mustBeNotNull == null) {				
				to_log("NULL is detected: " + message);
			}
		}						
		
		private void make_non_static_calls_activity() {
			to_log("make_non_static_calls_activity.10");
			try { //works
				_ActivityObject.Call("testVoid"); 
				_ActivityObject.Call("testString", "s1"); 
				String sresult = _ActivityObject.Call<String>("testStringString", "s2"); 				 
				to_log ("SUCCESS:  make_non_static_calls_activity10:" + sresult);
			} catch (Exception ex) {
				to_log("make_non_static_calls_activity10:" + ex.Message);
			}
			to_log("make_non_static_calls_activity.20");
			try { //works, calls NON STATIC functions correctly
				_ActivityObject.CallStatic("testVoid");  
				_ActivityObject.CallStatic("testString", "s1"); 
				String sresult = _ActivityObject.CallStatic<String>("testStringString", "s3"); 
				to_log ("SUCCESS:  make_non_static_calls_activity20:" + sresult);
			} catch (Exception ex) {
				to_log("make_non_static_calls_activity20:" + ex.Message);
			}

			to_log("make_non_static_calls_activity.30");
						try { 
							_ActivityClass.CallStatic("testVoid"); //doesn't work
							to_log ("SUCCESS:  make_non_static_calls_activity30");
						} catch (Exception ex) {
							to_log("make_non_static_calls_activity30:" + ex.Message);
						}
			to_log("make_non_static_calls_activity.40");		
						try {
							_MyActivityClass.CallStatic("testVoid", _ActivityObject);  //doesn't work
							to_log ("SUCCESS:  make_non_static_calls_activity40");
						} catch (Exception ex) {
							to_log("make_non_static_calls_activity40:" + ex.Message);
						}
			to_log("make_non_static_calls_activity.50");
		}
		
		private void make_static_calls_activity() {
			to_log("make_static_calls_activity.10");			
						try {
							_ActivityClass.CallStatic("testVoidStatic");	 //doesn't work
							to_log ("SUCCESS:  make_static_calls_activity10");
						} catch (Exception ex) {
							to_log("make_static_calls_activity10:" + ex.Message);
						}		
			
			to_log("make_static_calls_activity.20");			
			try { //works
				_MyActivityClass.CallStatic("testVoidStatic");	//works
				_MyActivityClass.CallStatic("testStringStatic", "s1");	//works
				String sresult =_MyActivityClass.CallStatic<String>("testStringStringStatic", "s1");	//works
				to_log ("SUCCESS:  make_static_calls_activity20:" + sresult);
			} catch (Exception ex) {
				to_log("make_static_calls_activity20:" + ex.Message);
			}		
			to_log("make_static_calls_activity.30");
			
		}		

		private void make_non_static_calls_myclass() {
			try { //works
				_MyClassObject.Call("testVoid");
				_MyClassObject.Call("testString", "s1");
				String sresult = _MyClassObject.Call<String>("testStringString", "s1");
				to_log ("SUCCESS:  make_non_static_calls_myclass:" + sresult);
				
			} catch (Exception ex) {
				to_log("make_non_static_calls_myclass:" + ex.Message);
			}		
		}
		
		private void make_static_calls_myclass() {
			try { //works
				_MyClassClass.CallStatic("testVoidStatic");
				_MyClassClass.CallStatic("testStringStatic", "s1");
				String sresult = _MyClassClass.CallStatic<String>("testStringStringStatic", "s1");
				to_log ("SUCCESS:  make_static_calls_myclass:" + sresult);
			} catch (Exception ex) {
				to_log("make_static_calls_myclass:" + ex.Message);
			}		
		}		
		
		
		private void make_non_static_calls_activity_int_params() {
			to_log("make_non_static_calls_activity_int_params.10");
			try { 
				long iresult = _ActivityObject.Call<long>("testLong", 999L); 
				to_log ("SUCCESS:  make_non_static_calls_activity10:" + iresult);
			} catch (Exception ex) {
				to_log("make_non_static_calls_activity_int_paramsy10:" + ex.Message);
			}
			to_log("make_non_static_calls_activity_int_params.20");
			try { 
				int iresult = _ActivityObject.Call<int>("testInt", 999); 
				to_log ("SUCCESS:  make_non_static_calls_activity_int_params20:" + iresult);
			} catch (Exception ex) {
				to_log("make_non_static_calls_activity_int_params20:" + ex.Message);
			}

			to_log("make_non_static_calls_activity_int_params.30");
			try { 
				int iresult = _ActivityObject.Call<int>("testInteger", 999); 
				to_log ("SUCCESS:  make_non_static_calls_activity_int_params30:" + iresult);
			} catch (Exception ex) {
				to_log("make_non_static_calls_activity_int_params30:" + ex.Message);
			}
			to_log("make_non_static_calls_activity_int_params.40");
		}		
		
		
		private void make_calls_from_thread() {
			System.Threading.Thread t = new System.Threading.Thread(this.thread_proc);
			t.Start();
			t.Join();
		}
		
        private void thread_proc() {
			to_log("Thread:  static calls");
			try {
				make_static_calls_activity();
			} catch (Exception ex) {
				to_log("Thread: static calls failed: " + ex.Message);
			}				
			
			to_log("Thread: non static calls myclass ");
			try {
				make_non_static_calls_myclass();
			} catch (Exception ex) {
				to_log("Thread: non static calls myclass failed: " + ex.Message);
			}				
			
			to_log("Thread:  static calls myclass");
			try {
				make_static_calls_myclass();
			} catch (Exception ex) {
				to_log("Thread: static calls failed: " + ex.Message);
			}				
			
			to_log("Thread: non static calls");
			try {
				make_non_static_calls_activity();
			} catch (Exception ex) {
				to_log("Thread: non static calls failed: " + ex.Message);
			}				
        }		
	}
}


