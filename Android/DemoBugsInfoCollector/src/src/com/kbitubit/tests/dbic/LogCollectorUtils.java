package com.kbitubit.tests.dbic;

import java.util.List;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;

/** http://code.google.com/p/android-log-collector/ 
 * */
public final class LogCollectorUtils {    
    public static final String LOG_COLLECTOR_PACKAGE_NAME = "com.xtralogic.android.logcollector";//$NON-NLS-1$
    public static final String ACTION_SEND_LOG = "com.xtralogic.logcollector.intent.action.SEND_LOG";//$NON-NLS-1$
    public static final String EXTRA_SEND_INTENT_ACTION = "com.xtralogic.logcollector.intent.extra.SEND_INTENT_ACTION";//$NON-NLS-1$
    public static final String EXTRA_DATA = "com.xtralogic.logcollector.intent.extra.DATA";//$NON-NLS-1$
    public static final String EXTRA_ADDITIONAL_INFO = "com.xtralogic.logcollector.intent.extra.ADDITIONAL_INFO";//$NON-NLS-1$
    //public static final String EXTRA_SHOW_UI = "com.xtralogic.logcollector.intent.extra.SHOW_UI";//$NON-NLS-1$
    public static final String EXTRA_FILTER_SPECS = "com.xtralogic.logcollector.intent.extra.FILTER_SPECS";//$NON-NLS-1$
    public static final String EXTRA_FORMAT = "com.xtralogic.logcollector.intent.extra.FORMAT";//$NON-NLS-1$
    public static final String EXTRA_BUFFER = "com.xtralogic.logcollector.intent.extra.BUFFER";//$NON-NLS-1$
	
    public static void collectAndSendLog(final Context context){    	
        final PackageManager packageManager = context.getPackageManager();
        final Intent intent = new Intent(ACTION_SEND_LOG);
        List<ResolveInfo> list = packageManager.queryIntentActivities(intent, PackageManager.MATCH_DEFAULT_ONLY);
        final boolean isInstalled = list.size() > 0;
        
        if (!isInstalled){
            new AlertDialog.Builder(context)
            .setTitle(context.getString(R.string.app_name))
            .setIcon(android.R.drawable.ic_dialog_info)
            .setMessage(context.getString(R.string.log_collector_suggestion_to_install))
            .setPositiveButton(android.R.string.ok, new DialogInterface.OnClickListener(){
                public void onClick(DialogInterface dialog, int whichButton){
                	Utils.openUrl(context, "market://search?q=pname:" + LOG_COLLECTOR_PACKAGE_NAME, false);
                }
            })
            .setNegativeButton(android.R.string.cancel, null)
            .show();
        }
        else{
            new AlertDialog.Builder(context)
            .setTitle(context.getString(R.string.app_name))
            .setIcon(android.R.drawable.ic_dialog_info)
            .setMessage(context.getString(R.string.log_collector_instruction, GlobalConstants.EMAIL_CONTACT_US))
            .setPositiveButton(android.R.string.ok, new DialogInterface.OnClickListener(){
                public void onClick(DialogInterface dialog, int whichButton){
                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    intent.putExtra(EXTRA_SEND_INTENT_ACTION, Intent.ACTION_SENDTO);
                    intent.putExtra(EXTRA_DATA, Uri.parse("mailto:" + GlobalConstants.EMAIL_CONTACT_US));
                    intent.putExtra(EXTRA_ADDITIONAL_INFO, "HW: " + get_hardware_info(context));
                    intent.putExtra(Intent.EXTRA_SUBJECT
                    		, GlobalConstants.APP_UID + " failed report " + Utils.getAppBuild(context));                    
                    intent.putExtra(EXTRA_FORMAT, "time");
                    
                    //The log can be filtered to contain data relevant only to your app
                    if (! Version.IS_TEST) {
	                    String[] filterSpecs = new String[3];
	                    filterSpecs[0] = "AndroidRuntime:E";
	                    filterSpecs[1] = GlobalConstants.TAG_LOG + ":V";
	                    filterSpecs[2] = "*:S";
	                    intent.putExtra(EXTRA_FILTER_SPECS, filterSpecs);
                    }
                    
                    context.startActivity(intent);
                }
            })
            .setNegativeButton(android.R.string.cancel, null)
            .show();
        }
    }
		
    /** Получить информацию о данном девайсе в строковом виде*/
	protected static String get_hardware_info(Context context) {
		try {
			StringBuilder sb = new StringBuilder();
			sb.append("AW build: " + Utils.getAppBuild(context));
			sb.append("brand: " + get_str(android.os.Build.BRAND));
			sb.append("device: " + get_str(android.os.Build.DEVICE));
			sb.append("board: " + get_str(android.os.Build.BOARD));
			sb.append("cpu_abi: " + get_str(android.os.Build.CPU_ABI));
			sb.append("display: " + get_str(android.os.Build.DISPLAY));
			sb.append("manufacturer: " + get_str(android.os.Build.MANUFACTURER));
			sb.append("model: " + get_str(android.os.Build.MODEL));
			sb.append("product: " + get_str(android.os.Build.PRODUCT));
			sb.append("tags: " + get_str(android.os.Build.TAGS));
			sb.append("type: " + get_str(android.os.Build.TYPE));
			sb.append("ver.codename: " + get_str(android.os.Build.VERSION.CODENAME));
			sb.append("ver.incremental: " + get_str(android.os.Build.VERSION.INCREMENTAL));
			sb.append("ver.release: " + get_str(android.os.Build.VERSION.RELEASE));
			sb.append("ver.sdk: " + android.os.Build.VERSION.SDK_INT);

			return sb.toString();
		} catch (Exception ex) {
			Kernel.logError(ex, ErrorCodes.E2);
			return "get_hardware_info error";
		}
	}

	private static String get_str(String s) {
		return (s == null ? "" : s) + "\n";
	}
}
