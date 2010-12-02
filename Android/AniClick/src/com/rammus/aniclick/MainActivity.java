package com.rammus.aniclick;

import java.util.Timer;
import java.util.TimerTask;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Canvas;
import android.graphics.PixelFormat;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

public class MainActivity extends Activity {
	private static final String TAG_LOG = "rammus.com.aniclick.MainActivity";
	private final int FPS = 5; 
	private ScreenManager m_ScreenManager;
	private ScreenView m_MainView;
	private int m_WidgetId;
	private AnimationThread m_AnimationThread = new AnimationThread();

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        super.onCreate(savedInstanceState);
        getWindow().setFormat(PixelFormat.TRANSLUCENT);
        m_MainView = new ScreenView(this);
		setContentView(m_MainView);
        
        String srect = getIntent().getExtras().getString(AniWidgetProvider.PARAM_WIDGET_POSITION); //get position on the screen for clicked widget
        m_WidgetId = getIntent().getExtras().getInt(AniWidgetProvider.PARAM_WIDGET_ID); 
        m_ScreenManager = new ScreenManager(Utils.StrToRect(srect), this);

    //start animation
    	Timer timer = new Timer();
    	timer.scheduleAtFixedRate(m_AnimationThread, 1, (int)(1000 / FPS) );
    	
    	m_ScreenManager.SetCurrentFrameId(DataStorage.GetValue(this, m_WidgetId));
    }
    	
	@Override
	public boolean onTouchEvent(MotionEvent event) {
	//any click on the animation stops it		
		if (event.getAction() == MotionEvent.ACTION_DOWN) { 
		//save selected frame to preferences - this data will be used by widget
			DataStorage.SetValue(this, m_WidgetId, m_ScreenManager.GetCurrentFrameId());			
		//update image on widget
			Intent active = new Intent(this, AniWidgetProvider.class);
		    active.setAction(AniWidgetProvider.ACTION_UPDATE_WIDGET_CONTEXT);
		    Uri uri = Uri.parse(AniWidgetProvider.URI_SCHEME + "://widget#");
		    uri = uri.buildUpon()
		    	.appendQueryParameter("widget_id", String.valueOf(m_WidgetId))
		    	.build();
		    active.setData(uri);		    
		    this.sendBroadcast(active);	
	    //close this activity
			m_AnimationThread.cancel();
  			finish();
		}
		return super.onTouchEvent(event);
	}
	
	/* We use SurfaceView instead of View to 
	 * have possibility to update screen when we need (i.e. 25 times per second).
	 * Standard "postInvalidate" doesn't guarantee in-time update. 
	 * */
	private class ScreenView extends SurfaceView implements SurfaceHolder.Callback { 
    	SurfaceHolder m_SurfaceHolder;
		public ScreenView(Context context){
			super(context);
			m_SurfaceHolder = this.getHolder();
			m_SurfaceHolder.setFormat(PixelFormat.TRANSLUCENT); //make ScreenView transparent
			m_SurfaceHolder.addCallback(this);
		}
		
		public void surfaceCreated(SurfaceHolder holder) {
			Repaint(); //ATTENTION: it's allow us to avoid black screen flicking during activity start.
		}

		public void surfaceDestroyed(SurfaceHolder holder) {
		}		
		
		public void surfaceChanged(SurfaceHolder holder, int format, int w, int h) {
		}		

		@Override protected void onDraw(Canvas canvas) {
			super.onDraw(canvas);			
		}

		private void paint(Canvas canvas) {
			m_ScreenManager.Draw(canvas);
		}
		
		public void Repaint() {
			//we need synchronized access to m_SurfaceHolder.lockCanvas
			//because we call Repaint from different threads: 
			//from main thread and from auxiliary (AnimationThread) one.
			synchronized (m_SurfaceHolder) { 
				Canvas c = m_SurfaceHolder.lockCanvas();
				if (c != null) {
					try {
						super.draw(c);
						paint(c);
					} finally {
						m_SurfaceHolder.unlockCanvasAndPost(c);
					}
				} else {
					Log.e(TAG_LOG, "c is null!");
				}
			}
		}		
	}

	/* Thread that look over animation frames and command to draw frame by frame.
	 * */
	private class AnimationThread extends TimerTask {
		public AnimationThread() {
			 			
		}
					
		@Override 
		public void run() {
			m_ScreenManager.MakeAnimationStep(); 
			m_MainView.Repaint();
		}
				
		@Override
		public boolean cancel() {
			return super.cancel();
		}
	}
}