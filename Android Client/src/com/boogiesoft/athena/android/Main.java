package com.boogiesoft.athena.android;

import java.sql.Date;
import java.text.SimpleDateFormat;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Window;

public class Main extends Activity {
	
	private Activity mActivity;
	
	public static SyncUser syncUser;
	
	public static MessageManager messageManager;
	
	public static TextMessages textMessages;
	
	public static ActiveLog activeLog;
	
	public static Engine mEngine;
	
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mActivity = this;
        mEngine = new Engine(mActivity);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
    }
    
    @Override
    protected void onResume() {
    	super.onResume();
    	if(messageManager == null) {
        	messageManager = new MessageManager(mActivity);
        }
        if(syncUser == null) {
        	syncUser = new SyncUser();
        }
        if(activeLog == null) {
        	activeLog = new ActiveLog();
        }
        mEngine.doStartUp();
    }
    
    @Override
    protected void onPause() {
    	super.onPause();
    	mEngine.ExitApp();
    }
	
	
    
    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if(keyCode == KeyEvent.KEYCODE_BACK) {
        	switch(mEngine.CURRENT_PAGE) {
        	case Engine.CONSOLE_PAGE:
        		mEngine.showMsg("Session Saved");
        		return super.onKeyDown(keyCode, event);
        	case Engine.ADMIN_PAGE:
        	case Engine.LOG_PAGE:
        	case Engine.TEXT_PAGE:
        	case Engine.DAILY_PAGE:
        		mEngine.CURRENT_PAGE = Engine.CONSOLE_PAGE;
        		mEngine.consoleScreen();
        		return true;
        	default :
        		return super.onKeyDown(keyCode, event);
        	}
        }
        return super.onKeyDown(keyCode, event);
    }
    
    public static void doLog( String s, int i) {
    	Log.d("LOG", s+", "+i);
    }
    
    @SuppressLint("SimpleDateFormat")
	public String getTodayStamp() {
        SimpleDateFormat sqlFormat = new SimpleDateFormat("yyyy/MM/dd");
        Date todaysDate = new Date(0);
        String ts = sqlFormat.format(todaysDate);
        return ts;
    }
    
    
    
    
    
    
    
    
    
}




