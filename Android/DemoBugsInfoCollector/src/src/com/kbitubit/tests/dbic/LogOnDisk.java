package com.kbitubit.tests.dbic;

import java.io.IOException;
import java.io.PrintStream;

import android.content.Context;
import android.text.format.Time;

/** Лог на SD-карте. Только в отладочной версии - в релизе недопустим,
 * т.к. постоянные обращения к SD-карте съедают батарейку. */
public class LogOnDisk {
	public static final String LOG_FILE_NAME = "custom_log";
	private final java.io.FileOutputStream _FS;
	private final java.io.PrintStream _PS;
	private final android.text.format.Time _Time = new Time();	
	public LogOnDisk(Kernel kernel) throws IOException {
		_FS = kernel.openFileOutput(LOG_FILE_NAME, Context.MODE_PRIVATE);
		_PS = new PrintStream(_FS);
	}
	
	public synchronized void toLog(String message) {
		try {			
			_Time.setToNow();
			_PS.print(_Time.format("%H:%M:%S ") + android.os.Process.getThreadPriority(android.os.Process.myTid()) + " " + message + "\n");
			_PS.flush();
		} catch (Exception ex) {
			//nothing to do
		}
	}		
}
