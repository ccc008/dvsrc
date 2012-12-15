package com.kbitubit.tests.dbic;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import com.apsalar.sdk.Apsalar;

/** Helper class to work with Apsalar Library 
 * https://apsalar.com/app/sdk/ 
 * */
public class ApsalarStat {
	/** apsalar session should be initialized one time only - at the launch of application 
	 * remember if session was already initialized */
	private static boolean _SessionInitialized = false;
		
	/** check if apsalar using was allowed by the user */
	public static Boolean isApsalarEnabled(Context context) {
		final SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
		return prefs.contains(Kernel.SP_KEY_APSALAR_ENABLED) 
			? prefs.getBoolean(Kernel.SP_KEY_APSALAR_ENABLED, false)
			: null;
	}
	/** save desicion of user - allow/disallow using of apsalar */
	public static void setApsalarEnabled(Context context, boolean enabled) {
		final SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(context);
		final SharedPreferences.Editor editor = prefs.edit();
		editor.putBoolean(Kernel.SP_KEY_APSALAR_ENABLED, enabled);
		editor.commit();
	}
	public static Boolean isApsalarInitialized() {
		return _SessionInitialized;
	}
	
	/** initialise apsalar session.
	 * if application is launched first time then shown dialog 
	 * "Help us to improve the application, allow us to receive anonymous statistics of the application usage."
	 * save user decision.
	 * */
	@SuppressWarnings("unused") 
	public static final void initSession(final Activity activity) {
		try {
			if (! _SessionInitialized) {
				//if (Version.IS_DEBUG) return;
				
				Boolean enabled = isApsalarEnabled(activity);
				if (enabled == null) {
					new AlertDialog.Builder(activity)
				      .setMessage(activity.getString(R.string.apsalar_statistics_suggestion))
				      .setCancelable(false)
				      .setNegativeButton(activity.getString(android.R.string.no), new DialogInterface.OnClickListener() {						
							@Override public void onClick(DialogInterface dialog, int which) {
								setApsalarEnabled(activity, false);
							}
				      }).setPositiveButton(activity.getString(android.R.string.yes), new DialogInterface.OnClickListener() {						
							@Override public void onClick(DialogInterface dialog, int which) {
								setApsalarEnabled(activity, true);								
								start_session(activity);
								_SessionInitialized = true;
							}
				      }).show();					
					return;
				} else if (enabled) {
					start_session(activity);
					_SessionInitialized = true;
				}
			} else {
				//it isn't necessary to initialize apsalar session second time
			}
		} catch (Exception ex) {
			Kernel.logError(ex, ErrorCodes.E1);
		}
	}

	private static void start_session(final Activity activity) {
		//!TODO: set real api key and secret and uncomment next line
		//Apsalar.startSession(activity, Kernel.APSALAR_API_KEY, Kernel.APSALAR_SECRET);
	}
	
	/** log apsalar event with [optional] event attributes 
	 * Example:
	 * 		logEvent("event"
	 * 			, "param1name", "param1value"
	 * 			, "param2name", "param2value"
	 * 		);
	 * */
	@SuppressWarnings("unused")
	public static final void logEvent(String eventName, Object... args) {
		if (Version.IS_DEBUG || ! _SessionInitialized) {
			return;
		}
		try {
			Apsalar.event(eventName, args);
		} catch (Exception ex) {			
			//don't write exception to log to avoid infinity cyrcle
		}
	}
	
	public static void logEventError(int errorCode) {
		logEvent("error", "code", errorCode);		
	}	
}
