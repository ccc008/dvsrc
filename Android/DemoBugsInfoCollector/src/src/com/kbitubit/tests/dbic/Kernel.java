package com.kbitubit.tests.dbic;

import org.acra.annotation.ReportsCrashes;
import org.acra.ACRA;
import org.acra.ACRAConfiguration;
import org.acra.ReportingInteractionMode;
import org.acra.ReportField;

import android.app.Application;
import android.util.Log;

/**  
 * http://code.google.com/p/acra/
 * https://github.com/ACRA/acra/wiki/AdvancedUsage
 * https://github.com/ACRA/acra/wiki/BasicSetup
 * */ 
@ReportsCrashes(formKey="" //Kernel.GOOGLE_DOCS_FORM_KEY
		, mailTo = GlobalConstants.EMAIL_CONTACT_US
		, customReportContent = { 
			ReportField.APP_VERSION_CODE
			, ReportField.USER_COMMENT
			, ReportField.APP_VERSION_NAME
			, ReportField.ANDROID_VERSION
			, ReportField.BRAND
			, ReportField.PHONE_MODEL
			, ReportField.CUSTOM_DATA
			, ReportField.STACK_TRACE
//#startBASE			
			, ReportField.APPLICATION_LOG
//#endBASE			
		}
        , mode = ReportingInteractionMode.NOTIFICATION
//#startBASE
        , applicationLogFile = LogOnDisk.LOG_FILE_NAME
//#endBASE
        //resToastText = R.string.crash_toast_text // optional, displayed as soon as the crash occurs, before collecting data which can take a few seconds
        , resNotifTickerText = R.string.crash_notif_ticker_text
        , resNotifTitle = R.string.crash_notif_title
        , resNotifText = R.string.crash_notif_text
        //, resNotifIcon = android.R.drawable.stat_notify_error // optional. default is a warning sign
        , resDialogText = R.string.crash_dialog_text
        //, resDialogIcon = android.R.drawable.ic_dialog_info //optional. default is a warning sign
        //, resDialogTitle = R.string.crash_dialog_title // optional. default is your application name
        , resDialogCommentPrompt = R.string.crash_dialog_comment_prompt // optional. when defined, adds a user text field input with this text resource as a label
        , resDialogOkToast = R.string.crash_dialog_ok_toast // optional. displays a Toast message when the user accepts to send a report.
)
public final class Kernel extends Application { 
	@Override public void onCreate() {
		try {
			org.acra.ACRA.init(this);
//#startBASE        
			_LogOnDisk = new LogOnDisk(this);
//#endBASE
		} catch (Exception ex) {
			Kernel.logError(ex, ErrorCodes.E94);
		}
	}

	/** Mearsure time from start of application */
	public static final PerfMeter MainMeter = new PerfMeter();
	
//#startBASE
	/// Ring buffer stores N last messages in memory.
	private static LogOnDisk _LogOnDisk;
//#endBASE
	
	private static void log_to_internal_log(String message) {
//#startBASE	
		_LogOnDisk.toLog(message);		
//#endBASE
	}
	   
	/** Log info in TEST only. This log is excluded in Pro and Free version. */
	public static void logInfo(String message) {
//#startBASE
		Log.i(GlobalConstants.TAG_LOG, message);
		log_to_internal_log(message);
//#endBASE
	}
	/** Log info in all versions */
	public static void logAlways(String srcMessage) {
		Log.i(GlobalConstants.TAG_LOG, srcMessage);
		log_to_internal_log(srcMessage);
	}
	
	/** Log error: NO exceptin, NO apsalar code */
	public static void logError(String... messages) {
		String s = Utils.array2str(messages);
		Log.e(GlobalConstants.TAG_LOG, s);
		log_to_internal_log("Error:" + s);
	}
	/** Log error: apsalar code; NO exception */
	public static void logError(int errorCode) {
		log_to_internal_log("Error:" + String.valueOf(errorCode));
		handle_error_with_code(errorCode);
	}

	/** Log error: exception; NO apsalar code */
	public static void logError(Exception ex) {
		Log.e(GlobalConstants.TAG_LOG, ex.toString());
		log_to_internal_log("Error:" + ex.toString());
	}
	/** Log error: exception + apsalar code*/
	public static void logError(Exception ex, int errorCode) {
		Log.e(GlobalConstants.TAG_LOG, ex.toString());
		log_to_internal_log("Error:" + ex.toString());
		handle_error_with_code(errorCode);
	}
	/** Log error: exception + apsalar code + list of parameters */
	public static void logError(Exception ex, int errorCode, String... messages) {
		String s = Utils.array2str(messages) + ex.toString();
		Log.e(GlobalConstants.TAG_LOG, s);
		log_to_internal_log("Error:" + s);
		handle_error_with_code(errorCode);
	}
	/** log with time tic
	 * for testing performance
	 * TEST version only*/
	public static void logTic(String message) { 
//#startBASE
		Kernel.MainMeter.testLog(message);
		log_to_internal_log(message);
//#endBASE		
	}
	
	/** register error in Apsalar
	 * send silent error reports to google doc */
	private static void handle_error_with_code(int errorCode) {
		ApsalarStat.logEventError(errorCode);
		if (ApsalarStat.isApsalarInitialized() && ErrorCodes.isErrorInteresting(errorCode) ) {
			//apsalar is initialized; user has allowed us to collect info
			try {
				set_acra_silent_mode(true);
				org.acra.ACRA.getErrorReporter().putCustomData("errorCode", String.valueOf(errorCode));
				org.acra.ACRA.getErrorReporter().handleSilentException(null);
				//Thread.sleep(1000);
			} catch (Exception ex) {
				Kernel.logError(ex);
			} finally {
				set_acra_silent_mode(false);
			}
		}
	}

	public static void set_acra_silent_mode(boolean bSilent) {
		try {
			ACRAConfiguration config = ACRA.getConfig();
			config.setMode(bSilent ? ReportingInteractionMode.SILENT : ReportingInteractionMode.NOTIFICATION);
			config.setFormKey(bSilent ? GlobalConstants.GOOGLE_DOCS_FORM_KEY : "");
			config.setMailTo(bSilent ? "" : GlobalConstants.EMAIL_CONTACT_US);
			ACRA.setConfig(config);
			ACRA.getErrorReporter().setDefaultReportSenders();
			ACRA.getErrorReporter().removeAllReportSenders();
			ACRA.getErrorReporter().setReportSender(new org.acra.sender.GoogleFormSender(GlobalConstants.GOOGLE_DOCS_FORM_KEY) );
		} catch (Exception ex) {
			Kernel.logError(ex);
		}
	}				
}
