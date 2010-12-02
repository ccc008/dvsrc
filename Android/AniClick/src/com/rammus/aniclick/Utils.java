package com.rammus.aniclick;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Rect;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Vibrator;
import android.util.Log;

public class Utils {
	public static String RectToStr(Rect srcRect) {
		return String.valueOf(srcRect.left) + ";" 
			+ String.valueOf(srcRect.top) + ";" 
			+ String.valueOf(srcRect.right) + ";"
			+ String.valueOf(srcRect.bottom);		
	}
	
	public static Rect StrToRect(String srcStr) {
        Pattern p = Pattern.compile("(\\d+);(\\d+);(\\d+);(\\d+)");
        Matcher m = p.matcher(srcStr);       
        m.find();		
		return new Rect(Integer.valueOf(m.group(1).toString())
				, Integer.valueOf(m.group(2).toString())
				, Integer.valueOf(m.group(3).toString())
				, Integer.valueOf(m.group(4).toString()));
	}

	public static Bitmap Drawable2Bitmap(Drawable drawable) {
		int width = drawable.getIntrinsicWidth();
		int height = drawable.getIntrinsicHeight();
		Bitmap bmp = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
		drawable.mutate().setBounds(0, 0, width, height);
		Canvas c = new Canvas(bmp);
		drawable.draw(c);
		return bmp;
	}
}
