package com.rammus.aniclick;

import android.content.Context;
import android.content.SharedPreferences;


/* We remember clicked frame and display it as widget image.
 * We need it to demonstrate possibility to update widget image after activity finishing
 * */
public class DataStorage {
	public static final String PREFS_NAME = "com.rammus.AniClick";
	public static final String PREF_PREFIX_KEY = "frame";
	
	public static int GetValue(Context context, int appWidgetId) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, 0);
		String sframe_id = prefs.getString(PREF_PREFIX_KEY + appWidgetId, null);
        if (sframe_id != null) {
            return Integer.parseInt(sframe_id);
        } else {
            return 0;
        }	
    }
	
	public static void SetValue(Context context, int appWidgetId, int frameId) {
		SharedPreferences.Editor prefs = context.getSharedPreferences(PREFS_NAME, 0).edit();
		prefs.putString(PREF_PREFIX_KEY + appWidgetId, String.valueOf(frameId));
        prefs.commit();
	}

	public static void RemovePreferences(Context context, int appWidgetId) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, 0);
		String lookup_key = prefs.getString(PREF_PREFIX_KEY + appWidgetId, null);
		if (lookup_key != null) {
			SharedPreferences.Editor edit_prefs = context.getSharedPreferences(PREFS_NAME, 0).edit();
			edit_prefs.remove(PREF_PREFIX_KEY + appWidgetId);
			edit_prefs.commit();
		}		
	}
}
