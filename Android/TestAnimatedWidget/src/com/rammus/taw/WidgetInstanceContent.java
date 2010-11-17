//Copyright 2010 by Victor Derevyanko, dvpublic0@gmail.com
//http://code.google.com/p/dvsrc/
//http://derevyanko.blogspot.com/2010/10/android-how-to.html
//{$Id$}
package com.rammus.taw;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.drawable.Drawable;

public class WidgetInstanceContent {
	public int FrameId = 0;
	public boolean IsAnimationInProcess = false;
	public final int WidgetId;
	public final int WidgetWidth;
	public final int WidgetHeight;
	
	public static int WHEEL_ANIMATION[] = {
		R.drawable.a1, R.drawable.a2, R.drawable.a3, R.drawable.a4
		, R.drawable.a5, R.drawable.a6, R.drawable.a7, R.drawable.a8
		, R.drawable.a9, R.drawable.a10, R.drawable.a11
	};
	public static int GetCountFrames() {
		return WHEEL_ANIMATION.length;
	}
	public static Bitmap CACHE_BITMAPS[] = new Bitmap[WHEEL_ANIMATION.length];
	
	public WidgetInstanceContent(int widgetId, int widgetWidth, int widgetHeight, Context context) {
		this.WidgetId = widgetId;
		this.WidgetHeight = widgetHeight;
		this.WidgetWidth = widgetWidth;
		//cache all drawables
		for (int i = 0; i < GetCountFrames(); ++i) {
			Drawable drw = context.getResources().getDrawable(WHEEL_ANIMATION[i]);
			CACHE_BITMAPS[i] = drawable_to_bitmap(drw);
		}
		
	}

	public Bitmap GetBitmapForCurrentFrame() {
		return CACHE_BITMAPS[this.FrameId];	
	}
	
	//id1 is [1..11]
	public static Bitmap GetCachedBitmapById(int id1) {
		return  CACHE_BITMAPS[id1 - 1];
	}
	
	  private static Bitmap drawable_to_bitmap(Drawable drw) {
		  Bitmap bmp = Bitmap.createBitmap(
		          drw.getIntrinsicWidth(), drw.getIntrinsicHeight(), Bitmap.Config.ARGB_8888);
		  Canvas canvas = new Canvas(bmp);
		  drw.mutate().setBounds(0, 0, drw.getIntrinsicWidth(), drw.getIntrinsicHeight());
		  drw.draw(canvas);
		  return bmp;
	  }		
}

