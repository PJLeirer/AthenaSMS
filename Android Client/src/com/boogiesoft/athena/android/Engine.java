package com.boogiesoft.athena.android;

import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.graphics.Color;
import android.os.AsyncTask;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.inputmethod.InputMethodManager;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.PopupWindow;
import android.widget.Spinner;
import android.widget.SpinnerAdapter;
import android.widget.TextView;
import android.widget.Toast;

import com.boogiesoft.athena.xfer.txDailyHoldTexts;

public class Engine {
	
	public Socket mSocket;
	public SockMainRecv sockRecv;
	public SockMainSend sockSend;
	
	public EditText loginUserField;
	public EditText loginPassField;
	public Button loginButton;
	public Button cancelButton;
	public Button resetButton;
	public Button adminMenuButton;
	public Button textMenuButton;
	public Button dailyTextButton;
	public Button logButton;
	public Button exitButton;
	public Button leaveTexting;
	public Button leaveDaily;
	public Button sendHolds;
	public Button leaveLog;
	public Button groupMessage;
	public Button changeLog;
	
	public TextView logView;
	
	public Spinner groupSelector;
	public Spinner logSelector;
	public ArrayAdapter<CharSequence> groupSpinnerAdapter;
	public ArrayAdapter<CharSequence> logSpinnerAdapter;
	
	public EditText txtGroupMsg;
	public EditText logTextFilter;
	public EditText configHostField;
	
	public PopupWindow popWin;
	
	private SharedPreferences smsConfig;
	
	private StartUp mStartUp;
	
	private Activity mActivity;
	
	private InputMethodManager inputManager;
	
	public int CURRENT_PAGE = 0x000000;
	public static final int CONFIG_PAGE = 0x000001;
	public static final int LOGIN_PAGE = 0x000002;
	public static final int CONSOLE_PAGE = 0x000004;
	public static final int TEXT_PAGE = 0x000008;
	public static final int DAILY_PAGE = 0x0000016;
	public static final int LOG_PAGE = 0x0000032;
	public static final int ADMIN_PAGE = 0x0000064;
	
	public String srvHost;
	public int srvPort;
	
	private TouchHandler mTouchHandler;
	
	private AlertDialog.Builder builder;
	
	private boolean isShuttingDown = false;
	
	
	public Engine(Activity act) {
		mActivity = act;
		mTouchHandler = new TouchHandler();
		smsConfig = mActivity.getPreferences(Activity.MODE_PRIVATE);
        inputManager = (InputMethodManager)mActivity.getSystemService(Context.INPUT_METHOD_SERVICE); 
	}
	
	
    
    public void doStartUp() {
        srvHost = smsConfig.getString("smsHost", null);
        srvPort = smsConfig.getInt("smsPort", 0);
        if(srvHost == null || srvPort == 0) {
        	configScreen();
        	showMsg("No Config Info");
        } else {
        	//SmsClient.doLog("Trying "+srvHost+":"+srvPort, 0);
        	mActivity.runOnUiThread(new Runnable() {
				@Override
				public void run() {
					mStartUp = new StartUp();
		        	mStartUp.execute("Starting Up");
				}
        	});
        }
    }
	
    private class StartUp extends AsyncTask<String, String, String> {
		
		@Override
		protected void onPreExecute() {
		   super.onPreExecute();
			mActivity.setContentView(R.layout.load);
		}
		
		@Override
		protected String doInBackground(String... params) {
			try {
				mSocket = new Socket(srvHost, srvPort);
				if(mSocket.isConnected()) {
					Main.doLog("sock connected", 0);
			        sockRecv = new SockMainRecv(mSocket);
			        sockSend = new SockMainSend(mSocket);
					sockRecv.start();
			        sockSend.start();
			        if(Main.syncUser.isLoggedIn()) {
			        	String uname = smsConfig.getString("smsUser", "");
			        	String upass = smsConfig.getString("smsPass", "");
			        	if(uname.length()>0 && upass.length()>0) {
			        		loginUser(uname, upass);
			        	} else {
			        		loginScreen();
			        	}
			        } else {
			        	loginScreen();
			        	Main.doLog("loginScreen", 0);
			        }
			        showMsg("Connected!");
				} else {
					showMsg("Unable to Connect to "+srvHost+"!");
					configScreen();
					return null;
				}
			} catch (Exception ex) {
				showMsg("Unable to Connect to "+srvHost+"!");
				Main.doLog("Exception caught: "+ex.getMessage(), 0);
				configScreen();
			}
			return null;
		}
		
		@Override
		protected void onPostExecute(String result) {
		   super.onPostExecute(result);
		   //nothing yet
		}
		
	}
    
    
        
    public void showMsg(final String msg) {
    	mActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				Toast.makeText(mActivity, msg, Toast.LENGTH_SHORT).show();
			}
    	});
    }
    

    
    public void loginUser(String uName, String uPass) {
    	if(sockSend.login(uName, uPass)) {
    		SharedPreferences.Editor editConfig = smsConfig.edit();
    		editConfig.putString("smsUser", uName);
    		editConfig.putString("smsPass", uPass);
    		editConfig.commit();
    		consoleScreen();
    		showMsg("Welcome "+uName);
    	} else {
			showMsg("Authorization Failed!");
    	}
    }
    
    public void logoutUser() {
    	SharedPreferences.Editor editConfig = smsConfig.edit();
    	editConfig.putString("smsUser", "");
    	editConfig.putString("smsPass", "");
    	editConfig.commit();
    	Main.syncUser = null;
    }

    
    public void deleteConfig() {
    	SharedPreferences.Editor killConfig = smsConfig.edit();
    	killConfig.clear();
    	killConfig.commit();
    	showMsg("Deleted Config");
    	doStartUp();
    }
    
    public void editConfig(String host) {
    	SharedPreferences.Editor editConfig = smsConfig.edit();
    	editConfig.putString("smsHost", host);
    	editConfig.putInt("smsPort", 10420);
    	editConfig.commit();
    }
    
    
    
    
    
    
    
    //// popups windows
    
    public void sendDailyHolds() {
    	
    	LayoutInflater vi = (LayoutInflater) mActivity.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    	View pv = vi.inflate(R.layout.confirm_send_holds, null);
    	popWin = new PopupWindow(pv, 400, 200, true);
		TextView cq = (TextView)pv.findViewById(R.id.confirmHolds);
		cq.setText("Whoa! This job is normally automated! Are you absolutely positive you want to process todays 'Daily Holds' file?");
		popWin.setOutsideTouchable(false);
		popWin.showAtLocation(mActivity.findViewById(R.id.msgsOut), Gravity.CENTER, 20, 20);
		Button confirmBtn = (Button)pv.findViewById(R.id.confirmRunHoldsButton);
		confirmBtn.setOnTouchListener(mTouchHandler);
		Button cancelBtn = (Button)pv.findViewById(R.id.cancelRunHoldsButton);
		cancelBtn.setOnTouchListener(mTouchHandler);
    }
    
    
    
    
    public void doGroupText() {
    	int minTxt = 3;
    	int p = groupSelector.getSelectedItemPosition();
    	String m = txtGroupMsg.getText().toString();
    	if(p>=0) {
    		if(m.length()>=minTxt) {
    			LayoutInflater vi = (LayoutInflater)mActivity.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    			View pv = vi.inflate(R.layout.confirm_group_text, null);
    			popWin = new PopupWindow(pv, 400, 200, true);
    			TextView cq = (TextView)pv.findViewById(R.id.confirmQuestion);
    			cq.setText("Are you sure you want to send \n'"+m+"'\n to  group '"+(String)groupSpinnerAdapter.getItem(p)+"'");
    			popWin.setOutsideTouchable(false);
    			popWin.showAtLocation(mActivity.findViewById(R.id.groupName), Gravity.CENTER, 20, 20);
    			Button confirmBtn = (Button)pv.findViewById(R.id.confirmGroupTextButton);
    			confirmBtn.setOnTouchListener(mTouchHandler);
    			Button cancelBtn = (Button)pv.findViewById(R.id.cancelGroupTextButton);
    			cancelBtn.setOnTouchListener(mTouchHandler);
        	} else {
        		showMsg("Message Not Long enough");
        	}
    	} else {
    		showMsg("No Group Selected");
    	}
    	
    }
    
    
    public void ExitApp() {
    	if(isShuttingDown) {
    		return;
    	}
    	isShuttingDown = true;
        sockRecv = null;
        sockSend = null;
    	try {
    		if(mSocket != null) {
    			mSocket.close();
    		}
		} catch (Exception e) {
		}
		mSocket = null;
        mActivity.finish();
    }
    
    
    
    
    
    
    
    ///////////// SCREENS
    ///////
    
    public void configScreen() {
		CURRENT_PAGE = CONFIG_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
    			mActivity.setContentView(R.layout.config);
    	    	configHostField = (EditText)mActivity.findViewById(R.id.hostentry);
    	    	Button configHostButton = (Button)mActivity.findViewById(R.id.createbutton);
    	    	configHostButton.setOnTouchListener(mTouchHandler);
			}
    	});
    }
    
    public void loginScreen() {
		CURRENT_PAGE = LOGIN_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
    			mActivity.setContentView(R.layout.login);
    	    	loginUserField = (EditText)mActivity.findViewById(R.id.userentry);
    	        loginPassField = (EditText)mActivity.findViewById(R.id.passentry);
    	    	loginButton = (Button)mActivity.findViewById(R.id.login_button);
    	    	loginButton.setOnTouchListener(mTouchHandler);
    	    	cancelButton = (Button)mActivity.findViewById(R.id.cancel_button);
    	    	cancelButton.setOnTouchListener(mTouchHandler);
    	    	resetButton = (Button)mActivity.findViewById(R.id.reset_button);
    	    	resetButton.setOnTouchListener(mTouchHandler);
			}
    	});
    }
    
    public void consoleScreen() {
		CURRENT_PAGE = CONSOLE_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
	    		mActivity.setContentView(R.layout.console);
	    		adminMenuButton = (Button)mActivity.findViewById(R.id.adminMenu);
		    	adminMenuButton.setOnTouchListener(mTouchHandler);
		    	textMenuButton = (Button)mActivity.findViewById(R.id.textMenu);
		    	textMenuButton.setOnTouchListener(mTouchHandler);
		    	dailyTextButton = (Button)mActivity.findViewById(R.id.dailyTexts);
		    	dailyTextButton.setOnTouchListener(mTouchHandler);
		    	logButton = (Button)mActivity.findViewById(R.id.log_button);
		    	logButton.setOnTouchListener(mTouchHandler);
		    	exitButton = (Button)mActivity.findViewById(R.id.exit_button);
		    	exitButton.setOnTouchListener(mTouchHandler);
    		}
    	});
    }
    
    public void textingScreen() {
		CURRENT_PAGE = TEXT_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
    			mActivity.setContentView(R.layout.texting);
    	    	groupSelector = (Spinner)mActivity.findViewById(R.id.groupName);
    	    	groupSpinnerAdapter = ArrayAdapter.createFromResource(mActivity, R.array.group_names, android.R.layout.simple_spinner_item);
    	    	groupSpinnerAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
    		    groupSelector.setAdapter(groupSpinnerAdapter);
    		    txtGroupMsg = (EditText)mActivity.findViewById(R.id.txtMessage);
    	    	leaveTexting = (Button)mActivity.findViewById(R.id.textingBackButton);
    	    	leaveTexting.setOnTouchListener(mTouchHandler);
    	    	groupMessage = (Button)mActivity.findViewById(R.id.sendGroup);
    	    	groupMessage.setOnTouchListener(mTouchHandler);
    		}
    	});
    }
    
    public void dailyScreen() {
		CURRENT_PAGE = DAILY_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
    			mActivity.setContentView(R.layout.daily);
    	    	TextView outMsgs = (TextView)mActivity.findViewById(R.id.outTotal);
    	    	TextView inMsgs = (TextView)mActivity.findViewById(R.id.inTotal);
    	    	outMsgs.setText(Integer.toString(Main.activeLog.todaysOutgoing));
    	    	inMsgs.setText(Integer.toString(Main.activeLog.todaysIncoming));
    	    	leaveDaily = (Button)mActivity.findViewById(R.id.dailyBackButton);
    	    	leaveDaily.setOnTouchListener(mTouchHandler);
    	    	sendHolds = (Button)mActivity.findViewById(R.id.dailySendHoldsButton);
    	    	sendHolds.setOnTouchListener(mTouchHandler);
    		}
    	});
    }
    
    public void showLogScreen(final String filter, final String direction) {
		CURRENT_PAGE = LOG_PAGE;
    	mActivity.runOnUiThread(new Runnable() {
    		@Override
			public void run() {
    			mActivity.setContentView(R.layout.showlog);
		    	showMsg("showLog: "+filter+", "+direction);
		    	logView = (TextView)mActivity.findViewById(R.id.logViewer);
		    	sockSend.getLog(filter, direction);
		    	logView.setText(Main.activeLog.showLog(null));
		    	logTextFilter = (EditText)mActivity.findViewById(R.id.logFilter);
		    	logTextFilter.addTextChangedListener(new TextWatcher() {
					@Override
					public void afterTextChanged(Editable s) {
						//
					}
					@Override
					public void beforeTextChanged(CharSequence s, int start, int count, int after) {
						//
					}
					@Override
					public void onTextChanged(CharSequence s, int start, int before, int count) {
						logView.clearComposingText();
		    	    	logView.setText(Main.activeLog.showLog(s).trim());
					}
		        });
		    	final CharSequence[] items = {"Outgoing", "Incoming"};
		    	builder = new AlertDialog.Builder(mActivity);
		    	builder.setTitle("Pick a direction");
		    	builder.setItems(items, new DialogInterface.OnClickListener() {
		    	    public void onClick(DialogInterface dialog, int item) {
		    	        showMsg(items[item].toString());
		    	        String D = "out";
		    	        if(items[item].toString().equals("Incoming")) {
		    	        	D = "in";
		    	        }
		    	    	sockSend.getLog(logTextFilter.getText().toString(), D);
		    	    	logView.clearComposingText();
		    	    	logView.setText(Main.activeLog.showLog(null).trim());
		    	    }
		    	});
		    	changeLog = (Button)mActivity.findViewById(R.id.logDirectionButton);
		    	changeLog.setOnTouchListener(mTouchHandler);
		    	leaveLog = (Button)mActivity.findViewById(R.id.logBackButton);
		    	leaveLog.setOnTouchListener(mTouchHandler);
    		}
    	});
    }
    
    
    public void adminScreen() {
    	if(Main.syncUser.getUserLevel()>=8) {
			CURRENT_PAGE = ADMIN_PAGE;
	    	mActivity.runOnUiThread(new Runnable() {
	    		@Override
				public void run() {
		    		mActivity.setContentView(R.layout.admin);
		    		
		    		List<String> userList = new ArrayList<String>();
		    		
		    		// get list
		    		
		    		
		    		SpinnerAdapter userAdapter = new ArrayAdapter<String>(mActivity, android.R.layout.simple_spinner_item);
		    		Spinner userSpin = (Spinner)mActivity.findViewById(R.id.spn_user_list);
		    		userSpin.setAdapter(userAdapter);
		    		
		    		
		    		
		    		//aButton = (Button)mActivity.findViewById(R.id.adminMenu);
			    	//aButton.setOnTouchListener(mTouchHandler);
			    	
	    		}
	    	});
    	} else {
    		showMsg("You need to be an Admin!");
    	}
    }
    
    
    
    //// handler
    
    public class TouchHandler implements OnTouchListener {
		@Override
		public boolean onTouch(View v, MotionEvent event) {
			Button b = (Button)v;
			switch(event.getAction()) {
			case MotionEvent.ACTION_DOWN:
				b.setBackgroundResource(R.drawable.buttonlt);
				b.setTextColor(Color.BLACK);
				break;
			case MotionEvent.ACTION_CANCEL:
			case MotionEvent.ACTION_OUTSIDE:
				b.setBackgroundResource(R.drawable.buttonbg);
				b.setTextColor(Color.rgb(205, 248, 242));
				break;
			case MotionEvent.ACTION_UP:
					b.setBackgroundResource(R.drawable.buttonbg);
					b.setTextColor(Color.rgb(205, 248, 242));
					switch(b.getId()) {
					case R.id.confirmRunHoldsButton:
						popWin.dismiss();
						popWin = null;
						consoleScreen();
				    	//textMessages = new TextMessages();
				    	txDailyHoldTexts DT = new txDailyHoldTexts();
				    	DT.runHolds = true;
				    	sockSend.sendDailyHoldTexts(DT);
				    	showMsg("Sending");
						break;
					case R.id.cancelRunHoldsButton:
						popWin.dismiss();
						popWin = null;
						break;
					case R.id.confirmGroupTextButton:
						int p = groupSelector.getSelectedItemPosition();
    					String name = (String)groupSpinnerAdapter.getItem(p);
    					String m = txtGroupMsg.getText().toString();
    					showMsg("Sending \n'"+m+"'\nto "+name);
    					sockSend.textGroup(p, name, m);
    					txtGroupMsg.setText(null);
    					popWin.dismiss();
    					popWin = null;
						break;
					case R.id.cancelGroupTextButton:
						popWin.dismiss();
    					popWin = null;
						break;
					case R.id.createbutton:
						String host = configHostField.getText().toString();
    					if(host.length()>0) {
    						editConfig(host);
    						doStartUp();
    					}
						break;
					case R.id.login_button:
						String luf = loginUserField.getText().toString();
    			    	String lpf = loginPassField.getText().toString();
    			    	if(luf.length()>0) {
    			    		if(lpf.length()>0) {
    			    			loginUser(luf, lpf);
    			    		} else {
    			    			showMsg("No Password Entered!");
    			    		}
    			    	} else {
    			    		showMsg("No Username Entered!");
    			    	}
						break;
					case R.id.cancel_button:
						logoutUser();
						ExitApp();
						break;
					case R.id.reset_button:
						deleteConfig();
						break;
					case R.id.textMenu:
						textingScreen();
						break;
					case R.id.dailyTexts:
						dailyScreen();
						break;
					case R.id.log_button:
						showLogScreen(null, "out");
						break;
					case R.id.exit_button:
						logoutUser();
						ExitApp();
						break;
					case R.id.textingBackButton:
						consoleScreen();
						break;
					case R.id.sendGroup:
						doGroupText();
						break;
					case R.id.dailyBackButton:
						consoleScreen();
						break;
					case R.id.dailySendHoldsButton:
						sendDailyHolds();
						break;
					case R.id.logDirectionButton:
						AlertDialog alert = builder.create();
						alert.show();
						break;
					case R.id.logBackButton:
						consoleScreen();
						break;
						
						//admin
					case R.id.adminMenu:
						adminScreen();
						break;
					}
					inputManager.hideSoftInputFromWindow(v.getWindowToken(), InputMethodManager.HIDE_NOT_ALWAYS);
				}
			return true;
		}
    }
}
