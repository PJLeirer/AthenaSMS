package com.boogiesoft.athena.android;

import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.List;

import com.boogiesoft.athena.xfer.txDailyHoldTexts;
import com.boogiesoft.athena.xfer.txData;
import com.boogiesoft.athena.xfer.txFunx;
import com.boogiesoft.athena.xfer.txGetLog;
import com.boogiesoft.athena.xfer.txGrpTxt;
import com.boogiesoft.athena.xfer.txUserLogin;

public class SockMainSend extends Thread {
	
    public ObjectOutputStream objOutStream = null;
    public Socket sock = null;
    public boolean doRun = true;

    SockMainSend(Socket s) {
        this.sock = s;
    }

    public boolean login(String U, String P) {
        System.out.println("Logging in.....");
        txUserLogin UL = new txUserLogin();
        UL.user = U;
        UL.pass = P;
        try {
            System.out.println("sending login request");
            objOutStream = new ObjectOutputStream(sock.getOutputStream());
            objOutStream.writeObject(UL);
            Main.syncUser.wait4Input();
            objOutStream = null;
            if(Main.syncUser.isLoggedIn()) {
                return true;
            } else {
                return false;
            }
        } catch (Exception e) {
            return false;
        }
    }

    public void textGroup(int id, String name, String txt) {
        txGrpTxt GT = new txGrpTxt();
        GT.ID = id;
        GT.NAME = name;
        GT.TXT = txt;
        try{
            objOutStream = new ObjectOutputStream(sock.getOutputStream());
            objOutStream.writeObject(GT);
            objOutStream = null;
        } catch (Exception e) {
            System.out.println("textGroup Failed!");
        }
    }
    
    public void getLog(String f, String d) {
        System.out.println("getLog(f d): direction is "+d);
        txGetLog GL = new txGetLog();
        GL.direction = d;
        GL.filter = f;
        try{
            objOutStream = new ObjectOutputStream(sock.getOutputStream());
            objOutStream.writeObject(GL);
            Main.activeLog.wait4Input();
            objOutStream = null;
            Main.activeLog.showLog(null);
        } catch (Exception e) {
            System.out.println("textGroup Failed!");
        }
        GL = null;
    }
    
    public void sendDailyHoldTexts(txDailyHoldTexts DT) {
    	try {
    		objOutStream = new ObjectOutputStream(sock.getOutputStream());
            objOutStream.writeObject(DT);
            objOutStream = null;
    	} catch(Exception ex) {
    		System.out.println("sendDailyHoldTexts Failed!");
    	}
    }
    
    public void getUserList() {
    	txFunx FX = new txFunx();
    	FX.FUNX = txFunx.USER_LIST;
    	try {
    		objOutStream = new ObjectOutputStream(sock.getOutputStream());
            objOutStream.writeObject(FX);
            objOutStream = null;
    	} catch(Exception ex) {
    		System.out.println("getUserList Failed!");
    	}
    	
    }
    
    
    
}




