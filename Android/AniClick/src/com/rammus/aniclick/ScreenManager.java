package com.rammus.aniclick;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Rect;
import android.util.DisplayMetrics;
import android.util.Log;

public class ScreenManager {
    public static final int[] IMAGES = {
    	R.drawable.a1, R.drawable.a2
    	, R.drawable.a3, R.drawable.a4
    	, R.drawable.a5, R.drawable.a6
    	, R.drawable.a7, R.drawable.a8
    	, R.drawable.a9, R.drawable.a10
    	, R.drawable.a11
    };
	
	private int m_CurrentFrame = 0; //current frame of animation
	private Rect m_WidgetRect; //position of clicked widget on the screen
	
	private Activity m_Activity;
	
	public ScreenManager(Rect srcWidgetRect,Activity parentActivity) {
		m_Activity = parentActivity;

		//Subtract size of title bar from rect
		int title_bar_height = get_titlebar_height(m_Activity) + 1; //!TODO: это подгоночный параметр; иначе картинка смещается относительно виджита
	    m_WidgetRect = new Rect(srcWidgetRect.left
	    		, srcWidgetRect.top - title_bar_height
	    		, srcWidgetRect.right
	    		, srcWidgetRect.bottom - title_bar_height
	    );    
	}

	public void Draw(Canvas canvas) {
	//clear background
		canvas.drawARGB( 100 + (int) 100 * m_CurrentFrame / (IMAGES.length - 1), 255, 255, 255);
	//draw current frame
		Bitmap bmp = Utils.Drawable2Bitmap(m_Activity.getResources().getDrawable(IMAGES[m_CurrentFrame]));
		
		canvas.drawBitmap(bmp
				, (int)(m_WidgetRect.left + m_WidgetRect.width()/2 - bmp.getWidth() / 2)
				, (int)(m_WidgetRect.top + m_WidgetRect.height()/2 - bmp.getHeight() / 2)
				, null);		
	}

	public void MakeAnimationStep() {
		m_CurrentFrame++;
		if (m_CurrentFrame > IMAGES.length - 1) {
			m_CurrentFrame = 0;			
		}
	}
	
	private static int get_titlebar_height( Activity parentActivity) {
		//http://stackoverflow.com/questions/3600713/size-of-android-notification-bar-and-title-bar
		DisplayMetrics metrics = new DisplayMetrics();
		parentActivity.getWindowManager().getDefaultDisplay().getMetrics(metrics);
		switch (metrics.densityDpi) {
	    	case DisplayMetrics.DENSITY_HIGH:
	            return 48;
	        case DisplayMetrics.DENSITY_LOW:
	            return 32;
	        case DisplayMetrics.DENSITY_MEDIUM:
	            return 24;
	        default:
	        	Log.i("display", "Unknown density");
	            return 0; 
		}
	}

	public int GetCurrentFrameId() {
		return m_CurrentFrame;
	}	
	
	public void SetCurrentFrameId(int frameId) {
		m_CurrentFrame = frameId;
	}		
}
