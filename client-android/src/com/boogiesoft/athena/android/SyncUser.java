package com.boogiesoft.athena.android;


public class SyncUser {
    private int userID = 0;
    private String userName = null;
    private int userLevel = 0;
    private boolean loggedIn = false;
    public synchronized boolean isLoggedIn() {
        if(loggedIn && userID>0 && userLevel>0 && userName != null) {
            return true;
        } else {
            if(! loggedIn) {
                System.out.println("loggedIn is false");
            }
            if(userID<1) {
                System.out.println("userID less than 1");
            }
            if(userLevel<1) {
                System.out.println("userLevel less than 1");
            }
            if(userName==null) {
                System.out.println("userName is null");
            }
            return false;
        }
    }
    public synchronized int getUserID() {
        return userID;
    }
    public synchronized String getUserName() {
        return userName;
    }
    public synchronized int getUserLevel() {
        return userLevel;
    }
    //used by sender
    public synchronized void wait4Input() {
        try {
            System.out.println("waiting...");
            wait();
            System.out.println("notified! exiting!");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    //used by reader
    public synchronized void assignUserInfo(int ID, String Name, int Level) {

        System.out.println("assigning local user info");
        this.userID = ID;
        this.userName = Name;
        this.userLevel = Level;
        if(ID>0) {
            loggedIn = true;
        }
        System.out.println("notifying..");
        notify();
    }
}
