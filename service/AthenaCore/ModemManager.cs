using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class ModemManager
    {

        public ArrayList myModems;
        public Stack outgoingMessages;
        public int modemCount = 0;
        public bool doRun = false;
        public String amReady = "Not Running!";

        public Core mCore;


        public ModemManager(Core core)
        {
            mCore = core;
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
                IModem l = (IModem)myModems[i];
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

        private void processOutgoingMessages()
        {

            while (outgoingMessages.Count > 0)
            {
                String[] msg = (String[])outgoingMessages.Pop();
                if (msg[0] != null)
                {
                    if (msg[0].Equals("sysmsg"))
                    {
                        mCore.doLogFile("System Message: " + msg[1], 0); // send email and log sysmsg
                        mCore.sendMail("Athena System Message", msg[1]);

                        // send msg to all connected clients
                        for (int i = 0; i < mCore.mSocketManager.mRecievers.Count; i++)
                        {
                            if (mCore.mSocketManager.mRecievers[i] != null)
                            {
                                SockReciever recv = (SockReciever)mCore.mSocketManager.mRecievers[i];
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
            Console.WriteLine("addModem(" + portName + ")");
            lock (myModems.SyncRoot)
            {

                // create modem
                IModem modem;
                if (portName.StartsWith("API"))
                {
                    // change this to custom modem class
                    modem = new ApiModem(mCore, portName, modemCount);
                }
                else
                {
                    modem = new CdmaModem(mCore, portName, modemCount);
                }
                

                if (modem.IsInstalledProperly() && modem.IsModemReady())
                {
                    myModems.Add(modem);
                    mCore.doEventLog("ModemManager.addModem: Adding Modem#" + modemCount + " on " + portName, 2);
                    //LogModem.modemLogs.add(modemCount, new Stack());
                    modemCount++;
                }
                else
                {
                    mCore.doEventLog("ModemManager.addModem: Modem Failed to install properly", 0);
                }
            }
            Console.WriteLine("end of addModem");
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
                    IModem modem = (IModem)myModems[i];
                    if (modem.IsModemReady() && !modem.IsOffhook())
                    {
                        m = i;
                        i = myModems.Count;
                    }
                }

                if (m >= 0)
                {
                    //if (Resources._debug) { Console.WriteLine(num + "|" + msg); }
                    IModem modem = (IModem)myModems[m];
                    modem.sendSMS(num, msg);
                }
                else
                {
                    mCore.doEventLog("ModemManager.sendSmsToModem(): Unable to get available modem", 0);
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
                    Resources.doEventLog("ModemManager.looper: "+e.Message+"\r\n"+e.StackTrace, 1);
                }
            }
            if (Resources._debug) { Console.WriteLine("leaving modem manager"); }
        }
        */



    }
}
