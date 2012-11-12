package com.example.u2aTest;

import android.app.Application;
import android.util.Log;

public final class Kernel extends Application {
	public static final String TAG_LOG = "com.example.u2aTest";
	
	@Override public void onCreate() {
		Kernel.logInfo("Kernel.onCreate");	
		super.onCreate();
	}

	@Override public void onTerminate() {
		Kernel.logInfo("Kernel.onTerminate");
		super.onTerminate();
	}
	
	public static void logInfo(String srcMessage) {
//#startBASE
		log_i(srcMessage);
//#endBASE
	}

	public static void logAlways(String srcMessage) {
		log_i(srcMessage);
	}

	/** Ошибки пишутся в лог, т.к. пользователи лог нам могут прислать */
	public static void logError(Exception ex, String srcMessage, int errorCode) {
		log_e(srcMessage + ex.toString());
	}
	public static void logError(Exception ex, int errorCode) {
		//!TODO: ApsalarStat.logError(ex, errorCode);
		log_e(ex.toString());
	}
	public static void logError(Exception ex) {
		//!TODO: ApsalarStat.logError(ex, 0);
		log_e(ex.toString());
	}
	public static void logError(Throwable ex) {
		//!TODO: ApsalarStat.logError(ex, 0);
		log_e(ex.toString());
	}
	public static void logError(String message) {
		log_e(message);	
	}
	
	private static void log_e(String message) {
		Log.e(TAG_LOG, message);
	}
	private static void log_i(String message) {
		Log.i(TAG_LOG, message);
	}	
}
