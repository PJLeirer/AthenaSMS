using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaService
{
    class ModemManager
    {

        public ArrayList myModems;
        public Stack outgoingMessages;
        public int modemCount = 0;
        public bool doRun = false;
        public String amReady = "Not Running!";


        public ModemManager()
        {
            myModems = new ArrayList();
            outgoingMessages = new Stack();
            doRun = true;
            amReady = "Running!";
        }

        public void ShutDown()
        {
            doRun = false;
            for (int i = 0; i < myModems.Count; i++)
            {
                CdmaModem l = (CdmaModem)myModems[i];
                l.ShutDown();
            }
            //myModems = null;
        }

        public void addToOutgoingMessages(ArrayList tm, String[] what)
        {
            lock (outgoingMessages.SyncRoot)
            {
                for (int i = 0; i < tm.Count; i++)
                {
                    String[] xm = (String[])tm[i];
                    string[] upd = new string[] { xm[0], xm[1] };
                    outgoingMessages.Push(upd);

                }
                what[1] += " Completed. " + tm.Count + " Messages total.";
                outgoingMessages.Push(what);

                //should only be called from here
                processOutgoingMessages();
            }
        }

        public void processOutgoingMessages()
        {

            while (outgoingMessages.Count > 0)
            {
                String[] msg = (String[])outgoingMessages.Pop();
                if (msg[0] != null)
                {
                    if (msg[0].Equals("sysmsg"))
                    {
                        Program.doLogFile("System Message: " + msg[1], 0); // send email and log sysmsg
                        Program.sendMail("Athena System Message", msg[1]);

                        // send msg to all connected clients
                        for (int i = 0; i < Program.mSocketManager.mRecievers.Count; i++)
                        {
                            if (Program.mSocketManager.mRecievers[i] != null)
                            {
                                SockReciever recv = (SockReciever)Program.mSocketManager.mRecievers[i];
                                recv.mSender.sendSysMsg("System Message", msg[1]);
                            }
                        }
                    }
                    else
                    {
                        sendSmsToModem(msg[0], msg[1]);
                    }
                }
            }

        }

        public void addModem(String portName)
        {
            lock (myModems.SyncRoot)
            {
                CdmaModem modem = new CdmaModem(portName, modemCount);
                if (modem.installedProperly && modem.modemReady)
                {
                    myModems.Add(modem);
                    Program.doEventLog("ModemManager.addModem: Adding Modem#" + modemCount + " on " + portName, 2);
                    //LogModem.modemLogs.add(modemCount, new Stack());
                    modemCount++;
                }
                else
                {
                    Program.doEventLog("ModemManager.addModem: Modem Failed to install properly", 0);
                }
            }
        }



        private void sendSmsToModem(String num, String msg)
        {
            lock (myModems.SyncRoot)
            {
                int m = -1;

                for (int i = 0; i <= myModems.Count; i++)
                {
                    if (i == myModems.Count)
                    {
                        i = 0;
                    }
                    CdmaModem modem = (CdmaModem)myModems[i];
                    if (modem.modemReady && !modem.offHook)
                    {
                        m = i;
                        i = myModems.Count;
                    }
                }

                if (m >= 0)
                {
                    //if (Program._debug) { Console.WriteLine(num + "|" + msg); }
                    CdmaModem modem = (CdmaModem)myModems[m];
                    modem.sendSMS(num, msg);
                }
                else
                {
                    Program.doEventLog("ModemManager.sendSmsToModem(): Unable to get available modem", 0);
                }
            }
        }


        /*
        public void start()
        {
            //new Thread(looper).Start();
        }
        
        private void looper()
        {
            doRun = true;
            while (doRun)
            {
                try
                {
                    Thread.Sleep(5000);
                    processOutgoingMessages();
                }
                catch (Exception e)
                {
                    Program.doEventLog("ModemManager.looper: "+e.Message+"\r\n"+e.StackTrace, 1);
                }
            }
            if (Program._debug) { Console.WriteLine("leaving modem manager"); }
        }
        */



    }
}
