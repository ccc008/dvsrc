package com.kbitubit.tests.dbic;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.net.Uri;

public final class Utils {
	/** Получить текущую версию приложения в строковом виде (для печати) */
	public static String getAppVersion(Context context) {
	    try {
		    PackageManager localPackageManager = context.getPackageManager();
		    PackageInfo localPackageInfo = localPackageManager.getPackageInfo(context.getPackageName(), 0);
		    return localPackageInfo.versionName + "." + String.valueOf(localPackageInfo.versionCode); 
	    } catch (Exception ex) {
	    	return "";
	    }	
	}
	
	/** Получить текущий билд приложения */
	public static int getAppBuild(Context context) {
	    try {
		    PackageManager localPackageManager = context.getPackageManager();
		    PackageInfo localPackageInfo = localPackageManager.getPackageInfo(context.getPackageName(), 0);
		    return localPackageInfo.versionCode; 
	    } catch (Exception ex) {
	    	return 0;
	    }		
	}
	
	/** Convert first N strings of array to single string, lines are separated by '\n' character */
    public static String array2str(String[] messages, int count) {
		if (count == 0) {
			return ""; 
		} else if (count == 1) {
			return messages[0];
		} else {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < count; ++i) {
				sb.append(messages[i]);
				sb.append('\n');
			}
			return sb.toString();
		}    	
    }
	/** Convert all strings of array to single string, lines are separated by '\n' character */
    public static String array2str(String[] messages) {
    	return array2str(messages, messages.length);
    }
	
	public static void openUrl(Context context, String url, boolean bNewTaskIsRequired) {
		Uri uri = Uri.parse(url);
		Intent intent  = new Intent(Intent.ACTION_VIEW, uri);
		if (bNewTaskIsRequired) {
			intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK); 
		}
		context.startActivity(intent);
	}
	
	
}


