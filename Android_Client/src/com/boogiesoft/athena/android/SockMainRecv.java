package com.boogiesoft.athena.android;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.net.Socket;

import com.boogiesoft.athena.xfer.txData;
import com.boogiesoft.athena.xfer.txFunx;
import com.boogiesoft.athena.xfer.txGetLog;
import com.boogiesoft.athena.xfer.txSysMsg;
import com.boogiesoft.athena.xfer.txUserInfo;

public class SockMainRecv extends Thread {
	
    public ObjectInputStream objInStream = null;
    public Socket mSocket = null;
    public boolean doRun = true;
    public String SystemMessage;
    
    SockMainRecv(Socket s) {
        mSocket = s;
    }

    @Override
    public void run() {
        while( doRun ) {
            try {
                System.out.println( "waiting for input..." );
                objInStream = null;
                objInStream = new ObjectInputStream( mSocket.getInputStream() ); // <- blocks thread
                final Object tmpObj = objInStream.readObject();
                
                    if(tmpObj instanceof txFunx) {
                        System.out.println("received txFunx");
                        //txFunx FX = (txFunx)tmpObj;
                    } else if(tmpObj instanceof txData) {
                        txData TD = (txData)tmpObj;
                        System.out.println("received txData: "+TD.jobTitle+" "+TD.jobFinished);
                        Main.textMessages.doConfirm(true);
                    } else if(tmpObj instanceof txUserInfo) {
                        System.out.println("received txUserInfo");
                        txUserInfo UI = (txUserInfo)tmpObj;
                        Main.syncUser.assignUserInfo(UI.userID, UI.userName, UI.userLevel);
                        if(UI.userID<=0 && UI.userName==null && UI.userLevel<=0) {
                        	Main.doLog("login failed", 0);
                        }
                    } else if(tmpObj instanceof txGetLog) {
                        txGetLog GL = (txGetLog)tmpObj;
                        System.out.println("received txGetLog");
                        Main.activeLog.setLog(GL.log, GL.todaysIncoming, GL.todaysOutgoing);
                    } else if(tmpObj instanceof txSysMsg) {
                        txSysMsg SM = (txSysMsg)tmpObj;
                        System.out.println("Received: "+SM.Msg);
                        Main.messageManager.addNewMsg(SM.Msg);
                    } else {
                        Main.doLog("Unknown Object Sent from "+mSocket.getRemoteSocketAddress()+"!", 0);
                    }
                
                if( mSocket.isInputShutdown() || mSocket.isClosed() ) {
                    doRun = false;
                }
            } catch (ClassNotFoundException ex) {
                doRun = false;
                Main.doLog("SockMainRecv.SockMainRecv(): ClassNotFound Exception, shutting down '"+this.getName()+"'", 1);
                //Main.unrecoverableError();
                try {
                    mSocket.close();
                } catch (Exception ex1) {
                    Main.doLog("Unable to close socket", 1);
                }
            } catch (IOException ex) {
                doRun = false;
                Main.doLog("SockMainRecv.run(): IO Exception, shutting down '"+this.getName()+"'", 1);
                Main.mEngine.ExitApp();
                try {
                    mSocket.close();
                } catch (Exception ex1) {
                    Main.doLog("Unable to close socket", 1);
                }
            }
        }
    }
	
	
}
