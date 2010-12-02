package com.rammus.aniclick;

import java.util.HashMap;
import java.util.Timer;
import java.util.TimerTask;

import com.rammus.aniclick.R;

import android.app.PendingIntent;
import android.appwidget.AppWidgetManager;
import android.appwidget.AppWidgetProvider;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Rect;
import android.net.Uri;
import android.provider.ContactsContract;
import android.util.Log;
import android.widget.RemoteViews;

public class AniWidgetProvider extends AppWidgetProvider {
	    private static final String TAG_LOG = "StartWidgetProvider";
	    private static final String ACTION_WIDGET_CONTROL = "com.rammus.aniclick.WIDGET_CONTROL";
	    public static final String ACTION_UPDATE_WIDGET_CONTEXT = "com.rammus.aniclick.UPDATE_WIDGET_CONTEXT";
	    public static final String URI_SCHEME = "com.rammus.flw";  
	    private static HashMap<Integer, WidgetInstanceManager> m_Instances = new HashMap<Integer, WidgetInstanceManager>();
	    
	    public static final String PARAM_WIDGET_ID = "com.rammus.aniclick.appWidgetId";
	    public static final String PARAM_WIDGET_POSITION = "com.rammus.aniclick.widgetPos";
		   
	    @Override
	    public void onUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds) {
	        for (int widget_id : appWidgetIds) {        	
	        	RemoteViews remote_views = new RemoteViews(context.getPackageName(), R.layout.widget);                	
	        	WidgetInstanceManager wim = new WidgetInstanceManager(widget_id, context);

	        	m_Instances.put(widget_id, wim);
	        	
						Intent active = new Intent(context, AniWidgetProvider.class);
		    		active.setAction(ACTION_WIDGET_CONTROL);
				    Uri data = Uri.parse(URI_SCHEME + "://widget#");
				    data = data.buildUpon()
				    	.appendQueryParameter("widget_id", String.valueOf(widget_id))
				    	//.appendQueryParameter("cell_id", 0)) //!TODO: additional info
			  	  	.build();
			   		active.setData(data);
			    	PendingIntent pi = PendingIntent.getBroadcast(context, 0, active, 0);
			    	remote_views.setOnClickPendingIntent(R.id.widget_image_view, pi); 
	        	
	        	update_widget_view(appWidgetManager, widget_id, remote_views);	        
	        }                  
	        super.onUpdate(context, appWidgetManager, appWidgetIds);	        
	    }   
	    
		@Override
	    public void onDeleted(Context context, int[] appWidgetIds) {   	
	        for (int widget_id : appWidgetIds) {    
	            //!TODO: remove preference
	        }
	        super.onDeleted(context, appWidgetIds);
	    }
	    
	    // Fix SDK 1.5 Bug per note here:
	    // http://developer.android.com/guide/topics/appwidgets/index.html#AppWidgetProvider
	    // linking to this post:
	    // http://groups.google.com/group/android-developers/msg/e405ca19df2170e2
	    @Override
	    public void onReceive(Context context, Intent intent) {
	        final String action = intent.getAction();
	        if (action.equals(AppWidgetManager.ACTION_APPWIDGET_DELETED)) {
	            final int appWidgetId = intent.getExtras().getInt(AppWidgetManager.EXTRA_APPWIDGET_ID, AppWidgetManager.INVALID_APPWIDGET_ID);
	            if (appWidgetId != AppWidgetManager.INVALID_APPWIDGET_ID) {
	                this.onDeleted(context, new int[] { appWidgetId });
	            }
	        } else if (action.equals(ACTION_WIDGET_CONTROL)) {
	        	Uri uri = intent.getData();
	        	Rect widget_pos = intent.getSourceBounds();
	        	int widget_id = Integer.parseInt(uri.getQueryParameter("widget_id"));
	            if (widget_id != AppWidgetManager.INVALID_APPWIDGET_ID) {
	            	make_widget_action(context, widget_id, widget_pos);
	            }	
	        } else if (action.equals(ACTION_UPDATE_WIDGET_CONTEXT)) {
	        	Uri uri = intent.getData();
	        	int widget_id = Integer.parseInt(uri.getQueryParameter("widget_id"));
	            if (widget_id != AppWidgetManager.INVALID_APPWIDGET_ID) {            	
	            	RemoteViews remote_views = new RemoteViews(context.getPackageName(), R.layout.widget);
	            	WidgetInstanceManager wim = get_instance(widget_id);
	            	wim.ReinitializeWidgetBitmap(context);
	           		update_widget_view(AppWidgetManager.getInstance(context), widget_id, remote_views);
	            } else super.onReceive(context, intent);	
	        } else {
	            super.onReceive(context, intent);
	        }
	    }    
	    
	  private void make_widget_action(Context context, int appWidgetId, Rect widgetPos) {
	    //start activity with animation
			Intent start = new Intent(context, MainActivity.class);
			start.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			start.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
			start.putExtra(PARAM_WIDGET_ID, appWidgetId); // key/value pair, where key needs current package prefix.
			start.putExtra(PARAM_WIDGET_POSITION, Utils.RectToStr(widgetPos)); // key/value pair, where key needs current package prefix.

			context.startActivity(start);
	  }
	    	   
		// find instance by id
		private WidgetInstanceManager get_instance(int appWidgetId) {
			return m_Instances.get(appWidgetId);
		}
	   
		private void update_widget_view(AppWidgetManager appWidgetManager, int appWidgetId, RemoteViews remoteViews) {
			WidgetInstanceManager wim = get_instance(appWidgetId);		
			remoteViews.setImageViewBitmap(R.id.widget_image_view, wim.GetBitmap() );
			appWidgetManager.updateAppWidget(appWidgetId, remoteViews);
		}
		
		private class WidgetInstanceManager {
			private Bitmap m_WidgetBitmap;
			public final int AppWidgetId;

			public WidgetInstanceManager(int appWidgetId, Context context) {
				this.AppWidgetId = appWidgetId;
				ReinitializeWidgetBitmap(context);		
			}
			
			public void ReinitializeWidgetBitmap(Context context) {
				int frame_id = DataStorage.GetValue(context, this.AppWidgetId);
				m_WidgetBitmap = BitmapFactory.decodeResource(context.getResources(), ScreenManager.IMAGES[frame_id]);
				Log.i(TAG_LOG, "widget were updated, frame: " + String.valueOf(frame_id));
			}

			public Bitmap GetBitmap() {				
				return m_WidgetBitmap;
			}
		}
		
	}