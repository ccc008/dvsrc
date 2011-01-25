package com.google.code.dvsrc;

import android.app.Activity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.View;
import android.widget.TextView;
import android.widget.ImageButton;

public class Main extends Activity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        this.prepareDonateButton_ProFree();
    }
    
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
	    MenuInflater inflater = getMenuInflater();
	    inflater.inflate(	      
	    	Version.GetProFreeId(R.menu.more_menu_free, R.menu.more_menu_pro) //AndroidProFreeSplitter will select required id 
	    	, menu);
	    return true;    
	}	
    
//#startBASE	
    public void prepareDonateButton_ProFree() {
    	//this function is required for base version only (for debug purposes)
    	//this function is never called in Pro/Free versions  
    	if (Version.IS_PRO) {
    		prepareDonateButton_Pro();
    	} else {
    		prepareDonateButton_Free();
    	}
    }
//#endBASE
    
 //#startFREE	
	public void prepareDonateButton_Free() {
	 	((TextView)this.findViewById(R.id.test_text))
	 		.setText(this.getResources().getString(R.string.hello_free));
		this.findViewById(R.id.test_image).setVisibility(View.VISIBLE); 
		ImageButton ib = (ImageButton)this.findViewById(R.id.test_image);
		ib.setBackgroundDrawable(this.getResources().getDrawable(R.drawable.donate));
	}
//#endFREE
	
//#startPRO	
	public void prepareDonateButton_Pro() {
		//hide donate button in Pro version
	 	((TextView)this.findViewById(R.id.test_text))
	 		.setText(this.getResources().getString(R.string.hello_pro));
		this.findViewById(R.id.test_image).setVisibility(View.GONE);
	}
//#endPRO	
    
} 