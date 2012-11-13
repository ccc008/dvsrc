package com.example.u2aTest;

/** Class with set of static and non-static functions for test purposes. */
public class MyClass {
	public MyClass() {
		Kernel.logInfo("MyClass.Constructor.No params");				
	}
	public MyClass(String anyParam) {
		Kernel.logInfo("MyClass.ConstructorWithParam:" + anyParam);				
	}
	
	public void testVoid() {
		Kernel.logInfo("MyClass.testVoid");		
	}

	public void testString(String paramValue) {
		Kernel.logInfo("MyClass.testString:" + paramValue);		
	}

	public String testStringString(String paramValue) {
		Kernel.logInfo("MyClass.testStringString:" + paramValue);
		return paramValue;
	}

	public Object testObjectObject(Object paramValue) {
		Kernel.logInfo("MyClass.testObjectObject:" + paramValue.toString());
		return paramValue;
	}
	public int testInt(int intValue) {
		Kernel.logInfo("MyClass.testInt:" + intValue);
		return intValue;
	}	
	
	
	public static void testVoidStatic() {
		Kernel.logInfo("MyClass.testVoidStatic");		
	}

	public static void testStringStatic(String paramValue) {
		Kernel.logInfo("MyClass.testStringStatic:" + paramValue);		
	}

	public static String testStringStringStatic(String paramValue) {
		Kernel.logInfo("MyClass.testStringStringStatic:" + paramValue);
		return paramValue;
	}
	public static Object testObjectObjectStatic(Object paramValue) {
		Kernel.logInfo("MyClass.testObjectObjectStatic:" + paramValue.toString());
		return paramValue;
	}
	public static int testIntStatic(int intValue) {
		Kernel.logInfo("MyClass.testIntStatic:" + intValue);
		return intValue;
	}	
}
