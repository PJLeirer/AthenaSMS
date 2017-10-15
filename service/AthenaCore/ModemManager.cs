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

                            mCore.doEventLog("System Message: " + msg[1], 2);
                            mCore.doNotify("Athena System Message", msg[1]);

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
                    IModem modem = (IModem)myModems[m];
                    modem.sendSMS(num, msg);
                    X = true;
                }
                else
                {
                    mCore.doEventLog("ModemManager.sendSmsToModem(): Unable to get available modem", 0);
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
                IModem modem = (IModem)myModems[i];
                if (modem.IsModemReady())
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
                    Resources.doEventLog("ModemManager.looper: "+e.Message+"\r\n"+e.StackTrace, 1);
                }
            }
            if (Resources._debug) { Console.WriteLine("leaving modem manager"); }
        }
        */



    }
}


/* SAVE
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
*/