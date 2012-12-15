package com.kbitubit.tests.dbic;

public class PerfMeter {
	private final long m_Start;
	private long m_Last;
	public PerfMeter() {
		m_Start = android.os.SystemClock.uptimeMillis();
		update_last();
	}
	public final long getResult() {
		return android.os.SystemClock.uptimeMillis() - m_Start;
	}
	public final void testLog(String str) {
		Kernel.logInfo(str + " " + getResult() + " " + get_delta());		
	}
	private long get_delta() {
		return android.os.SystemClock.uptimeMillis() - update_last();		
	}
	private long update_last() {
		long dest = m_Last;
		m_Last = android.os.SystemClock.uptimeMillis();
		return dest;
	}

}
