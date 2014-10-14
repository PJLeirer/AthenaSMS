AthenaSMS
=========

Text Messaging Solution


Please Read ALL of this file! It shouldn't be possible to do any damage but you should stay on the safe side and double check all settings (and this file) to be sure your modem wil work.
Use this software at Your Own Risk. I will not be held responsible for damage to modem hardware

There are several solutions to text messaging available. Athena is not designed to be a replacement or competition to these other solutions, but could possibly be, depending on your needs. This was written before a lot of them existed and never tried to copy or match what anyone else was doing. This code has sat around for almost a year as I do not have the time to finish it. I work from home as a developer and do not have the time to finish, package, market or promote it. Don't have the desire too anymore either, but I hate it see it go to waste. So, here it is for the open source world to have it. I hope it is useful many people. Hopefully people will want to contribute.

This project was a huge learning process for me and much of the code could be update to be more efficient.
You can laugh at some of the code if you want to, I do. But it works, I can at least say that!

This is a very DIY solution to running your own text messaging service and designed to
be used by professionals. Once installed properly it will work for anyone, you may need your 'computer guy' to do any troubleshooting if problems arise. You will need YOUR OWN MODEM. A multitech usb cdma wireless modem. (still looking for link to product)

It is recommended that Athena is installed by a professional, or at least someone with a good working knowledge of some windows administration and basic computer skills.

The server was designed to be a solution for adding text messaging to any project to connect as a client.
A windows client is supplied and works! If you wish to build or add-on your own client software, the socket classes for the client & server should be used as reference when building client classes for your project.

Sending Single and Group Texts works right-out-of-the-box, If you wish to use scheduled jobs, you will need to supply the files. This can be achieved by creating a file from a script or program, or create one manually at worst.

The format for batch files is: number|message<cr>(or)<cr><nl>, works for files created in unix/linux & windows

555-555-0000|Hello Jim
555-555-0100|Hello Bob
5555551234 | Hello World

all the above lines are valid.
a new line on the end should be fine but not sure
whitespace and dashes are removed from left of pipe.
whitespace is removed before first char from right of pipe.


Notes for Everyone:

The 'Windows Service' version is not currently up to date, mostly. it should be used as reference for the time being.
The NonService version is a Console Application and has all the latest updates.

You will to install cdma modem on a COM port.

You will need to create the folder C:\AthenaService with subfolders bin & conf. the .exe will run anywhere but recommended to run from bin folder.

On first run, a default .conf file will be created in the conf folder and event viewer application logging info is setup. Stop or Close the server. Open the conf file and edit all that apply.  Set the Com port to whatever port you installed the modem on. Multiple modems have not been tested but theoretically should work. If you wish to try 2 or more modems,  just copy/paste new ‘MODEM:COM?’ lines, there is no limit set.

Before you buy or install a modem, take a good look at the CdmaModem.cs file, particularly the init and shutdown ‘AT Command’ strings. These may be different per model. I highly advise checking that all the AT commands are the proper commands for your modem. They should be standard and compliant across the board

Open SqlDb.cs to check database setting. Create an MSSQL database and use what the class uses for a db name or edit class to use a different db name. Once the DB is setup and Athena is able to connect, all the tables will be created. Anytime a table is missing, Athena will re-recreate it on startup.

Check your Event Viewer if you have any problems, installing or otherwise. Info, Warnings & Errors are all logged in EventViewer under AtheneServiceLog in ‘Applications’.

I will update this more and maybe add more docs for communicating with the server.


Notes for Developers:

I'm bad at writing comments and usually work solo, so there is little to no comments, but the code should make sense

The last updates made to the client were in the ‘Reports’ Tab, it is not finished and only some parts are working.

The ‘Info’ & ‘Modem’ Tabs are only for testing socket & modem communications.

Have Fun with it. I hope it works out as a good solution for you. I encourage you to share your updates.

I'm new to github, so bare with me if it takes a moment for me to get up to speed.
