
package com.boogiesoft.athena.xfer;

import java.io.Serializable;


public class txGetLog implements Serializable{
	private static final long serialVersionUID = 1L;
    public String direction;
    public String filter;
    public String[][] log;
    public int todaysIncoming = 0;
    public int todaysOutgoing = 0;
}

