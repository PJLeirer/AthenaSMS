package com.boogiesoft.athena.xfer;

import java.io.Serializable;

public class txFunx implements Serializable {
	
    private static final long serialVersionUID = 1L;
        
    /// customizable members
        
    public static final int ADD_USER = 0x000000;
    public static final int USER_PASS = 0x000001;
    public static final int USER_LVL = 0x000002;
    public static final int USER_LIST = 0x000004;
        
    public int FUNX = -1;
    public String ARG0;
    public String ARG1;
    public int ARG2;
    
    
        
}
