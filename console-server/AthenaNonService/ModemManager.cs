/*
 * Modem Manager.
 * holds an array of CdmaModem instances, created based on conf file.
 * handles pool of outgoing messages
 * sends text messages to 1st available modem, modem is unavailable from the time a text is being sent until receipt from base is recieved.
 * incoming messages do not effect synchronization, or shouldn't.
 * all incoming and outgoing texts are logged in DB. sql mgmt studio should be used for best results. client has some log viewing features.
 * 
 */




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
            myModems = null;
        }



        public bool addToAndProcessOutgoingMessages(ArrayList tm, string[] what)
        {
            bool X = false;
            lock (outgoingMessages.SyncRoot)
            {

                what[1] += " Completed. " + tm.Count + " Messages total.";
                outgoingMessages.Push(what);

                // add to messages
                for (int i = 0; i < tm.Count; i++)
                {
                    String[] xm = (String[])tm[i];
                    string[] upd = new string[] { xm[0], xm[1] };
                    outgoingMessages.Push(upd);
                }


                // process messages
                while (outgoingMessages.Count > 0)
                {
                    String[] msg = (String[])outgoingMessages.Pop();
                    if (msg[0] != null)
                    {
                        if (msg[0].Equals("sysmsg"))
                        {

                            Program.doEventLog("System Message: " + msg[1], 2);
                            Program.doNotify("Athena System Message", msg[1]);

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
                            if (!sendSmsToModem(msg[0], msg[1]))
                            {
                                return false;
                            }
                        }
                    }
                }
                X = true;
            }
            return X;
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



        public bool sendSmsToModem(String num, String msg)
        {
            bool X = false;
            lock (myModems.SyncRoot)
            {

                System.Text.RegularExpressions.Regex.Replace(num, "[^0-9.]", "");

                Int16 m = getNextModem();
                if (m >= 0)
                {
                    Console.WriteLine(num + " | " + msg);
                    CdmaModem modem = (CdmaModem)myModems[m];
                    modem.sendSMS(num, msg);
                    X = true;
                }
                else
                {
                    Program.doEventLog("ModemManager.sendSmsToModem(): Unable to get available modem", 0);
                }

            }
            return X;
        }



        private Int16 getNextModem()
        {
            Int16 m = -1;
            if (myModems.Count < 1)
            {
                return m;
            }

            for (int i = 0; i <= myModems.Count; i++)
            {
                if (i == myModems.Count)
                {
                    i = 0;
                }
                CdmaModem modem = (CdmaModem)myModems[i];
                if (modem.modemReady)
                {
                    m = (Int16)i;
                    i = myModems.Count + 1;
                }
                else
                {
                    Thread.Sleep(20);
                }
            }

            return m;
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
