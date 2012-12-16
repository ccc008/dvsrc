package com.kbitubit.tests.demo;

import com.kbitubit.tests.dbic.ApsalarStat;
import com.kbitubit.tests.dbic.ErrorCodes;
import com.kbitubit.tests.dbic.Kernel;
import com.kbitubit.tests.dbic.LogCollectorUtils;
import com.kbitubit.tests.dbic.R;
import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class DemoActivity extends Activity {
	
	@Override protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		ApsalarStat.initSession(this);
		
		((Button)findViewById(R.id.message1)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				Kernel.logInfo("message 1");
			}
		});
		((Button)findViewById(R.id.message2)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				Kernel.logAlways("message 2");
			}
		});
		((Button)findViewById(R.id.error1)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				Kernel.logError(ErrorCodes.E3);
			}
		});
		((Button)findViewById(R.id.error2)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				Kernel.logError(ErrorCodes.E4);
			}
		});
		((Button)findViewById(R.id.crash)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				throw new RuntimeException("controllable crash!");
			}
		});
		((Button)findViewById(R.id.send_log)).setOnClickListener(new View.OnClickListener() {			
			public void onClick(View v) {
				LogCollectorUtils.collectAndSendLog(DemoActivity.this);
			}
		});
	}
}
