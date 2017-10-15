AthenaSMS
=========

Text Messaging Solution


Please Read ALL of this file! It shouldn't be possible to do any damage but you should stay on the safe side and double check all settings (and this file) to be sure your modem wil work.
Use this software at Your Own Risk. I will not be held responsible for damage to modem hardware


A mock modem is setup by default for testing. I've tested it with a multitech usb cdma wireless modem. (MTCBA-C-U-N3 https://www.multitech.com/models/92502842LF). All their modems should use the same AT commands but double check. the init & shutdown string may be different.

Athena will not currently restrict the amount of texts going in or out. An 'Unlimited Text' plan is recommended.
Athena only communicates with the modem(s). It will not setup service for your modem, you must call your cell phone provider and give them the id# on the modem.
Voice and Data are not required. (I don't know of any plans without voice, doesn't hurt to ask them tho).
Athena does not currently use any voice or data features that come with wireless modems. maybe some day.

Fees for texting are strictly between you and your cell phone service provider. Athena will not & can not see your cell phone service information and is intended to be completely seperate.
Athena will log Receipt Codes returned from your providers cell tower. this info can be useful for determining the cause of lost text messages or to verify that recipient recieved text. See SqlDb.cs for codes, also in your modem's manual.


The server was designed to be a solution for adding text messaging to any project to connect as a client.
A windows client is supplied and works! If you wish to build or add-on your own client software, the socket classes for the client & server should be used as reference when building client classes for your project.

Sending Single and Group Texts works right-out-of-the-box, If you wish to use scheduled jobs, you will need to supply the files. This can be achieved by creating a file from a script or program, or create one manually at worst.

The format for batch files is: number|message(cr)or(cr)(nl), works for files created in unix/linux & windows

555-555-0000|Hello Jim<cr>
555-555-0100|Hello Bob<cr>
5555551234 | Hello World<cr>

all the above lines are valid
a new line on the end should be fine but not sure. 
whitespace and dashes are removed from left of pipe.
whitespace is removed before first char from right of pipe.


Notes for Everyone:

You will need to install cdma modem on a COM port.

You will need to create the folder C:\AthenaService with subfolders bin, log & conf. the .exe will run anywhere but recommended to run from bin folder.

On first run, a default .conf file will be created in the conf folder and event viewer application logging info is setup. Stop or Close the server. Open the conf file and edit all that apply.  Set the Com port to whatever port you installed the modem on. Multiple modems have not been tested but theoretically should work. If you wish to try 2 or more modems,  just copy/paste new ‘MODEM:COM?’ lines, there is no limit set.

Before you buy or install a modem, take a good look at the CdmaModem.cs file, particularly the init and shutdown ‘AT Command’ strings. These may be different per model. I highly advise checking that all the AT commands are the proper commands for your modem. They should be standard and compliant across the board

Open SqlDb.cs to check database setting. Create an MSSQL database and use what the class uses for a db name or edit class to use a different db name. Once the DB is setup and Athena is able to connect, all the tables will be created. Anytime a table is missing, Athena will re-recreate it on startup.

Check your Event Viewer if you have any problems, installing or otherwise. Info, Warnings & Errors are all logged in EventViewer under AtheneServiceLog in ‘Applications’.

