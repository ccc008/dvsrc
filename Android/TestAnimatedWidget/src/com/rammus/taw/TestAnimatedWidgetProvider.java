//Copyright 2010 by Victor Derevyanko, dvpublic0@gmail.com
//http://code.google.com/p/dvsrc/
//http://derevyanko.blogspot.com/2010/10/android-how-to.html
//{$Id$}
package com.rammus.taw;

import java.util.HashMap;
import java.util.Timer;
import java.util.TimerTask;

import android.app.PendingIntent;
import android.appwidget.AppWidgetManager;
import android.appwidget.AppWidgetProvider;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.net.Uri;
import android.util.Log;
import android.widget.RemoteViews;

public class TestAnimatedWidgetProvider extends AppWidgetProvider{	
	//Flag to select variant of animation: static of dynamic.
	//1) false - pass to remote view identificator of next animation frame
	//2) true - pass to remote view "generated" bitmap. Bitmap is simply loaded from resources (см. WidgetInstanceContent)
	//variant 1) allowes ot use hight FPS. 
	//variant 2) has terribly bad performance and generates errors FAILED BINDER TRANSACTION
	//
	private static final boolean USE_GENERATED_BITMAPS = false; 
	private static final int FPS = 25; 
    
    private static final String LOG_TAG = "com.rammus.TAW";
    private static final String ACTION_WIDGET_CONTROL = "com.rammus.taw.WIDGET_CONTROL";
    private static final String EXTRA_APPWIDGET_ID = "com.rammus.taw.APP_WIDGET_ID";
    private static final String URI_SCHEME = "com.rammus.taw";
    private static HashMap<Integer, WidgetInstanceContent> m_Instances = new HashMap<Integer, WidgetInstanceContent>();
	
    @Override
    public void onUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds) {
    	Log.d(LOG_TAG, "onUpdate(): ");
   	    	
        for (int widget_id : appWidgetIds) {        	
        	RemoteViews remote_view = new RemoteViews(context.getPackageName(), R.layout.taw);
                	
        	WidgetInstanceContent wic = new WidgetInstanceContent(widget_id
        			, appWidgetManager.getAppWidgetInfo(widget_id).minWidth
        			, appWidgetManager.getAppWidgetInfo(widget_id).minHeight
        			, context);
        	
        //make widget clickable
			Intent active = new Intent(context, TestAnimatedWidgetProvider.class);
			active.setAction(ACTION_WIDGET_CONTROL);
		    Uri data = Uri.parse(URI_SCHEME + "://widget#");
		    data = data.buildUpon()
		    	.appendQueryParameter("widget_id", String.valueOf(widget_id))
		    	.build();
		    active.setData(data);
		    PendingIntent pi = PendingIntent.getBroadcast(context, 0, active, 0);
		    remote_view.setOnClickPendingIntent(R.id.bkview, pi);
        	
        	m_Instances.put(widget_id, wic);       	
        	update_widget_view(appWidgetManager, widget_id, remote_view);        	
        }                  
        super.onUpdate(context, appWidgetManager, appWidgetIds);    
    }   
        
	// find instance by id
	private WidgetInstanceContent get_instance(int appWidgetId) {
		return m_Instances.get(appWidgetId);
	}    
   
	@Override
    public void onDeleted(Context context, int[] appWidgetIds) {   	
        Log.d(LOG_TAG, "onDelete()");
//        for (int widget_id : appWidgetIds) {                       
//            Log.d(LOG_TAG, "Removing preference for id " + widget_id); // remove preference
//        }
        super.onDeleted(context, appWidgetIds);
    }
    
    // Fix SDK 1.5 Bug per note here:
    // http://developer.android.com/guide/topics/appwidgets/index.html#AppWidgetProvider
    // linking to this post:
    // http://groups.google.com/group/android-developers/msg/e405ca19df2170e2
    @Override
    public void onReceive(Context context, Intent intent) {
        final String action = intent.getAction();
        //Log.d(LOG_TAG, "OnReceive:Action: " + action);
        if (action.equals(AppWidgetManager.ACTION_APPWIDGET_DELETED)) {
            final int appWidgetId = intent.getExtras().getInt(AppWidgetManager.EXTRA_APPWIDGET_ID, AppWidgetManager.INVALID_APPWIDGET_ID);
            if (appWidgetId != AppWidgetManager.INVALID_APPWIDGET_ID) {
                this.onDeleted(context, new int[] { appWidgetId });
            }
        } else if (action.equals(ACTION_WIDGET_CONTROL)) {
            //final int widget_id = intent.getExtras().getInt(EXTRA_APPWIDGET_ID, AppWidgetManager.INVALID_APPWIDGET_ID);
        	Uri uri = intent.getData();
            final int widget_id = Integer.parseInt(uri.getQueryParameter("widget_id"));
            if (widget_id != AppWidgetManager.INVALID_APPWIDGET_ID) {
            	make_widget_action(context, AppWidgetManager.getInstance(context), widget_id);
            }	
        } else {
            super.onReceive(context, intent);
        }
    }    
    
    private void make_widget_action(Context context, AppWidgetManager instance, int appWidgetId) {
		//User has clicked on widget - make appropriate action
    	WidgetInstanceContent wic = get_instance(appWidgetId);
    	if (wic.IsAnimationInProcess) return; //ignore all clicks while animation is in process
    	
    	wic.IsAnimationInProcess = true;
    	wic.FrameId = 0;
    	AnimationThread at = new AnimationThread(context, instance, appWidgetId);

    	Timer timer = new Timer();
    	timer.scheduleAtFixedRate(at, 1, (int)(1000 / FPS) );    	
    }
   
	private void update_widget_view(AppWidgetManager appWidgetManager, int appWidgetId, RemoteViews remoteViews) {
		WidgetInstanceContent wic = get_instance(appWidgetId);

		if (! USE_GENERATED_BITMAPS) {
			int step = wic.FrameId;
			remoteViews.setImageViewResource(R.id.bkview, WidgetInstanceContent.WHEEL_ANIMATION[step]);
			appWidgetManager.updateAppWidget(appWidgetId, remoteViews);
		} else {
			Bitmap bmp = wic.GetBitmapForCurrentFrame();
			if (bmp != null)	 {
				remoteViews.setImageViewBitmap(R.id.bkview, bmp);
				appWidgetManager.updateAppWidget(appWidgetId, remoteViews);
			}
		}
	}
   
    private class AnimationThread extends TimerTask {
    	private final String LOG_TAG = "com.rammus.startwidget.AnimationThread"; 
		private RemoteViews m_RemoteViews;
		private AppWidgetManager m_AppWidgetManager;
		private int m_AppWidgetId;
		
		public AnimationThread(Context context, AppWidgetManager appWidgetManager, int appWidgetId) {
			m_AppWidgetManager = AppWidgetManager.getInstance(context); //appWidgetManager;
			m_AppWidgetId = appWidgetId;
			m_RemoteViews = new RemoteViews(context.getPackageName(), R.layout.taw);
		}
		
		@Override 
		public void run() {
			WidgetInstanceContent wic = get_instance(m_AppWidgetId);
			update_widget_view(m_AppWidgetManager, m_AppWidgetId, m_RemoteViews);
			if (wic.IsAnimationInProcess) {
				wic.FrameId++;
				if (wic.FrameId == WidgetInstanceContent.GetCountFrames()) {
					wic.FrameId = 0;
					wic.IsAnimationInProcess = false;
				}
			} else {
				cancel();
			}
		}
				
		@Override
		public boolean cancel() {
			//Log.i(LOG_TAG, "cancel");			
			return super.cancel();
		}
	}           

}
