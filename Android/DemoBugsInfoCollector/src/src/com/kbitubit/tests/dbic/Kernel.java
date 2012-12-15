package com.kbitubit.tests.dbic;

import org.acra.annotation.ReportsCrashes;
import org.acra.ReportingInteractionMode;
import org.acra.ReportField;

import android.app.Application;
import android.util.Log;

/** 
 * https://apsalar.com/app/sdk/ 
 * http://code.google.com/p/acra/
 * http://code.google.com/p/android-log-collector/
 * */ 
@ReportsCrashes(formKey="dDdabWZ5THM0a21KUHRuQlFzNGxtRXc6MQ"
		//mailTo = Kernel.EMAIL_CONTACT_US,
//		, customReportContent = { 
//			ReportField.APP_VERSION_CODE
//			, ReportField.USER_COMMENT
//			, ReportField.APP_VERSION_NAME
//			, ReportField.ANDROID_VERSION
//			, ReportField.BRAND
//			, ReportField.PHONE_MODEL
//			, ReportField.CUSTOM_DATA
//			, ReportField.STACK_TRACE
//		}
//        , mode = ReportingInteractionMode.SILENT,
//        //resToastText = R.string.crash_toast_text, // optional, displayed as soon as the crash occurs, before collecting data which can take a few seconds
//        resNotifTickerText = R.string.crash_notif_ticker_text,
//        resNotifTitle = R.string.crash_notif_title,
//        resNotifText = R.string.crash_notif_text,
//        //resNotifIcon = android.R.drawable.stat_notify_error, // optional. default is a warning sign
//        resDialogText = R.string.crash_dialog_text,
//        //resDialogIcon = android.R.drawable.ic_dialog_info, //optional. default is a warning sign
//        //resDialogTitle = R.string.crash_dialog_title, // optional. default is your application name
//        resDialogCommentPrompt = R.string.crash_dialog_comment_prompt, // optional. when defined, adds a user text field input with this text resource as a label
//        resDialogOkToast = R.string.crash_dialog_ok_toast // optional. displays a Toast message when the user accepts to send a report.
)
public final class Kernel extends Application { 
	public static final String TAG_LOG = "com.kbitubit.tests.dbic";
	public static final String EMAIL_CONTACT_US = "support on mydomain.com";
	public static final String APSALAR_API_KEY = "TODO";
	public static final String APSALAR_SECRET = "TODO";
	public static final String SP_KEY_APSALAR_ENABLED = "DBIC_SP_KEY_APSALAR_ENABLED";
	private static final int SIZE_LONG_RING_BUFFER = 150; //max 150 messages
	public static final String LOG_COLLECTOR_EMAIL_SUBJECT = "DBIC failed report";

	@Override public void onCreate() {
		try {
			org.acra.ACRA.init(this);
		} catch (Exception ex) {
			Kernel.logError(ex, ErrorCodes.E94);
		}
	}

	/** Mearsure time from start of application */
	public static final PerfMeter MainMeter = new PerfMeter();
	
//#startBASE
	/** Ring buffer stores N last messages in memory.*/
	private static final LogRingBuffer _LogRingBuffer = new LogRingBuffer(SIZE_LONG_RING_BUFFER);
//#endBASE
	
	private static void log_to_ring_buffer(String message) {
//#startBASE	
		_LogRingBuffer.append(message);		
//#endBASE
	}
	   
	/** Log info in TEST only. This log is excluded in Pro and Free version. */
	public static void logInfo(String message) {
//#startBASE
		Log.i(TAG_LOG, message);
		log_to_ring_buffer(message);
//#endBASE
	}
	/** Log info in all versions */
	public static void logAlways(String srcMessage) {
		Log.i(TAG_LOG, srcMessage);
		log_to_ring_buffer(srcMessage);
	}
	
	/** Log error: NO exceptin, NO apsalar code */
	public static void logError(String... messages) {
		String s = Utils.array2str(messages);
		Log.e(TAG_LOG, s);
		log_to_ring_buffer(s);
	}
	/** Log error: apsalar code; NO exception */
	public static void logError(int errorCode) {
		log_to_ring_buffer(String.valueOf(errorCode));
		handle_error_with_code(errorCode);
	}

	/** Log error: exception; NO apsalar code */
	public static void logError(Exception ex) {
		Log.e(TAG_LOG, ex.toString());
		log_to_ring_buffer(ex.toString());
	}
	/** Log error: exception + apsalar code*/
	public static void logError(Exception ex, int errorCode) {
		Log.e(TAG_LOG, ex.toString());
		log_to_ring_buffer(ex.toString());
		handle_error_with_code(errorCode);
	}
	/** Log error: exception + apsalar code + list of parameters */
	public static void logError(Exception ex, int errorCode, String... messages) {
		String s = Utils.array2str(messages) + ex.toString();
		Log.e(TAG_LOG, s);
		log_to_ring_buffer(s);
		handle_error_with_code(errorCode);
	}
	/** log with time tic
	 * for testing performance
	 * TEST version only*/
	public static void logTic(String message) { 
//#startBASE
		Kernel.MainMeter.testLog(message);
		log_to_ring_buffer(message);
//#endBASE		
	}
	
	/** register error in Apsalar
	 * send silent error reports to google doc */
	private static void handle_error_with_code(int errorCode) {
		ApsalarStat.logEventError(errorCode);
		if (ApsalarStat.isApsalarInitialized() && ErrorCodes.isErrorInteresting(errorCode) ) {
			//apsalar is initialized; user has allowed us to collect info
			String s = _LogRingBuffer.toString();
			org.acra.ACRA.getErrorReporter().putCustomData("errorCode", String.valueOf(errorCode));
			org.acra.ACRA.getErrorReporter().putCustomData("ring_buffer_content", s);
			org.acra.ACRA.getErrorReporter().handleException(null);
		}
	}				
}
