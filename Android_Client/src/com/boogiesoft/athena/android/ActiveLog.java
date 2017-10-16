package com.boogiesoft.athena.android;

public class ActiveLog {
	private String[][] log;
	public int todaysIncoming = 0;
	public int todaysOutgoing = 0;
	
	public synchronized void setLog(String s[][], int in, int out) {
		if(s != null) {
			log = s;
			todaysIncoming = in;
			todaysOutgoing = out;
		}
		notify();
	}

    public synchronized String showLog(CharSequence filter) {
        String logTxt = "";
        if(log != null) {
	        if(filter == null) {
	        	for(int i=0; i<log.length; i++) {
	            	logTxt += log[i][0]+"  |  "+log[i][2]+"  |  "+log[i][3]+"\n";
	        	}
	        } else {
	        	for(int i=0; i<log.length; i++) {
	        		if(log[i][0].contains(filter)) {
	        			System.out.println("Found match on row "+i);
	        			logTxt += log[i][0]+"  |  "+log[i][2]+"  |  "+log[i][3]+"\n";
	        		} else {
	        			System.out.println("No match for row "+i);
	        		}
	        	}
	        }
        }
        return logTxt;
    }
    
    public synchronized void wait4Input() {
        try {
            wait();
        } catch (InterruptedException e) {
            Main.doLog("ActiveLog.wait4input(): Interrupted Exception thrown", 1);
        }
    }
}
