
package com.boogiesoft.athena.android;


import android.app.Activity;
import android.widget.Toast;

/**
 *
 * @author pj
 */
public class MessageManager {
	
	Activity smsClient;
	public String newMsg;
	
	MessageManager(Activity mainApp) {
		smsClient = mainApp;
	}

    public void addNewMsg(String s) {
        this.newMsg = s;
        smsClient.runOnUiThread(new Runnable() {
            public void run() {
                Toast.makeText(smsClient, newMsg, Toast.LENGTH_SHORT).show();
            }
        });
    }

}

