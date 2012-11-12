package com.example.u2aTest;

import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

/** Main activity of Android plugin. 
 * It contains set of static and non-static functions for test purposes. 
 * */
public class MyMainActivity extends UnityPlayerActivity {
	@Override protected void onCreate(Bundle icicle) {	
		super.onCreate(icicle);
		Kernel.logInfo("MyMainActivity.onCreate");
	}
	
	@Override protected void onResume() {
		super.onResume();
		Kernel.logInfo("MyMainActivity.onResume");
		//com.unity3d.player.UnityPlayer.UnitySendMessage("GameObjectName1", "MethodName1", "Message to send");
	}
	
	@Override protected void onStop() {
		super.onStop();
		Kernel.logInfo("MyMainActivity.onStop");
	}

	@Override protected void onPause() {
		super.onStop();
		Kernel.logInfo("MyMainActivity.onPause");
	}
	
	
	
	public void testVoid() {
		Kernel.logInfo("MyMainActivity.testVoid");		
	}

	public void testString(String paramValue) {
		Kernel.logInfo("MyMainActivity.testString:" + paramValue);		
	}

	public String testStringString(String paramValue) {
		Kernel.logInfo("MyMainActivity.testStringString:" + paramValue);
		return paramValue;
	}

	public Object testObjectObject(Object paramValue) {
		Kernel.logInfo("MyMainActivity.testObjectObject:" + paramValue.toString());
		return paramValue;
	}
	
	
	
	public static void testVoidStatic() {
		Kernel.logInfo("MyMainActivity.testVoidStatic");		
	}

	public static void testStringStatic(String paramValue) {
		Kernel.logInfo("MyMainActivity.testStringStatic:" + paramValue);		
	}

	public static String testStringStringStatic(String paramValue) {
		Kernel.logInfo("MyMainActivity.testStringStringStatic:" + paramValue);
		return paramValue;
	}
	public static Object testObjectObjectStatic(Object paramValue) {
		Kernel.logInfo("MyMainActivity.testObjectObjectStatic:" + paramValue.toString());
		return paramValue;
	}
	
}
