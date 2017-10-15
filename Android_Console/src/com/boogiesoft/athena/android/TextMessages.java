package com.boogiesoft.athena.android;

public class TextMessages {

    private boolean sent = false;

    public boolean isConfirmed() {
        return sent;
    }

    private void confirmSent(boolean b) {
        sent = b;
    }

    public synchronized void wait4Input() {
        try {
            wait();
        } catch (InterruptedException e) {
            Main.doLog("TextMessages.wait4input threw Interrupted execption", 1);
        }
    }

    public synchronized void doConfirm(boolean b) {
        confirmSent(b);
        //notify();
    }

}

