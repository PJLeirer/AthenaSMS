/*
 * Modem Class. all modem comunications are handled here.
 * 
 * this class has been tested with an MTCBA-C1-U-N3 wireless modem from multitech.
 * any cdma modem should work with minor changes to this file.
 * this is the only file you will need to modify for different modems.
 * 
 * IMPORTANT! Be sure to read the modem's manual even if you have the same model! Be absolutely sure all AT commands are correct for the device!
 * 
 * 
 * */


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class CdmaModem : IModem
    {

        //debug
        private bool isMockModem = false; //arduino mock modem
        //private byte CTRL_W = (byte)0x17;
        //private byte SHIFT_OUT = (byte)0x0E;
        //private byte SHIFT_IN = (byte)0x0F;

        protected char CTRL_Z = (char)0x1A;

        // check your modem's init AT Command options, any changes must be the exact settings. these option setting are crucial.
        // I will add more info on these settings from the manual for MTCBA-C1-U-N3
        private string initCommand = "AT+CNMI=2,2,0,1,0";

        private int BAUD = 115200;
        private String portName = null;
        public string GetMyPortName()
        {
            return new String(portName.ToCharArray());
        }
        private bool modemReady = false;
        public bool IsModemReady()
        {
            return modemReady.Equals(true);
        }

        private bool offHook = false;
        public bool IsOffhook()
        {
            return offHook.Equals(true);
        }

        public SerialPort serialPort = null;

        public bool isReading = false;

        private String currentNumber = null;
        private String currentMessage = null;
        private int myNumber;
        public int GetMyNumber()
        {
            return (myNumber);
        }

        public String[] currentSms = new String[2];

        private bool installedProperly = false;
        public bool IsInstalledProperly()
        {
            return installedProperly.Equals(true);
        }

        public StreamWriter mModemLog;

        String[] seperator0 = new String[] { "\r\n\r\n" };
        String[] seperator1 = new String[] { "\r\n" };
        String[] seperator2 = new String[] { "," };
        String[] seperator3 = new String[] { " " };
        String[] seperator4 = new String[] { ":" };

        public Core mCore;


        public CdmaModem(Core core, String port, int num)
        {
            try
            {
                mCore = core;

                portName = port;
                myNumber = num;

                isMockModem = portName.StartsWith("DMY");

                if (!isMockModem)
                {
                    serialPort = new SerialPort(portName, BAUD, Parity.None, 8, StopBits.One);
                    serialPort.RtsEnable = true;
                    //serialPort.DtrEnable = true;
                    //serialPort.Handshake = Handshake.RequestToSend;
                    serialPort.ReadTimeout = 500;
                    serialPort.WriteTimeout = 500;

                    serialPort.Open();

                    serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                }



                //DateTime time = DateTime.Now;
                //string fn = time.ToString("MM-dd-yyyy");
                //mModemLog = new StreamWriter(Resources.AthenaDir + @"log\modem" + myNumber + "_" + fn, true);



                Thread.Sleep(1500);

                doModemLog("ATE0", 0);
                WriteToModem("ATE0");

                Thread.Sleep(900);

                doModemLog(initCommand, 0);
                WriteToModem(initCommand);

                installedProperly = true;
                modemReady = true;

            }
            catch (Exception e)
            {
                mCore.doEventLog("CdmaModem.initModem: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }

        protected void doModemLog(String s, int n) // 1=in, 0=out
        {
            try
            {
                DateTime myTime = DateTime.Now;
                string fn = myTime.ToString("MM-dd-yyyy");
                string ext = myTime.ToString("HH:mm:ss:ffff");
                mModemLog = new StreamWriter(Resources.AthenaDir + @"log\modem" + myNumber + "_" + fn, true);
                if (n > 0)
                {
                    mModemLog.WriteLine("<input>\n" + s + "\n</input>" + ext + "\n\n");
                }
                else
                {
                    mModemLog.WriteLine("<output>\n" + s + "\n</output>\n   @ " + ext + "\n\n");
                }
                mModemLog.Close();
            }
            catch (Exception e)
            {
                mCore.doEventLog("CdmaModem.doModemLog: " + e.Message + "\r\n" + e.StackTrace, 1);
            }
            finally
            {
                if (mModemLog != null)
                {
                    mModemLog.Close();
                }
            }
        }


        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs sdre)
        {
            SerialPort sp = (SerialPort)sender;
            StringBuilder sb = new StringBuilder();
            String input = "";
            try
            {
                do
                {
                    sb.Append(sp.ReadExisting());
                    Thread.Sleep(50); //wait in case of lag
                }
                while (sp.BytesToRead > 0);
                input = sb.ToString();



                // parse
                ParseModemResponse(input);
            }
            catch (Exception e)
            {
                mCore.doEventLog("CdmaModem.Reader: " + e.Message + "\r\n" + e.StackTrace, 0);
            }


        }


        public void ShutDown()
        {
            isReading = false;
            if (serialPort != null)
            {
                serialPort.Close();
            }
            
        }


        public void sendSMS(String num, String msg)
        {
            modemReady = false;
            offHook = true;
            currentNumber = num;
            currentMessage = msg;
            try
            {

                WriteToModem("AT+CMGS=\"" + num + "\"\r");
                //doModemLog("AT+CMGS=\"" + num + "\"", 0);
                //serialPort.Write("AT+CMGS=\"" + num + "\"\r");


                if (isMockModem)
                {
                    Thread.Sleep(900);
                }
                else
                {
                    Thread.Sleep(600);
                }

                doModemLog(msg + CTRL_Z, 0);
                WriteToModem(msg + CTRL_Z);

                Thread.Sleep(200);

                modemReady = true;
                offHook = false;
            }
            catch (Exception e)
            {
                mCore.doEventLog("CdmaModem.sendSMS(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }


        private void WriteToModem(string msg)
        {
            try
            {
                doModemLog(msg, 0);
                if(isMockModem)
                {
                    MockWrite(msg);
                }
                else
                {
                    serialPort.Write(msg);
                }
                
            }
            catch(Exception e)
            {
                doModemLog("Error: " + e.Message, 0);
            }
        }



        private void ParseModemResponse(string input)
        {
            doModemLog(input, 1);

            String[] inAT = input.Split(seperator0, StringSplitOptions.None);

            for (int i = 0; i < inAT.Length; i++)
            {

                if (inAT[i].Contains("+WANS"))
                {
                    modemReady = false;
                    offHook = true;
                }

                if (inAT[i].Contains("+CMGS"))
                {
                    //run to make sure it's splitting right
                    String[] cmgsInfo = inAT[i].Split(seperator4, StringSplitOptions.None);
                    mCore.mSqlDb.addOutgoingEntry(myNumber, cmgsInfo[1].Trim(), currentNumber, currentMessage);
                }

                if (inAT[i].Contains("ERROR"))
                {
                    modemReady = true;
                    offHook = false;
                    mCore.doEventLog("ERROR from modem on current text num: '" +
                            currentNumber + "' msg:" + currentMessage + " at " +
                            mCore.mSqlDb.getTimeStamp(), 0);
                    mCore.mSqlDb.failedOutgoing(currentNumber, currentMessage);
                }

                if (inAT[i].Contains("+CDS:"))
                {
                    modemReady = true;
                    offHook = false;
                    mCore.mSqlDb.updateOutgoingEntryReport(myNumber, inAT[i]);
                }

                if (inAT[i].Contains("+CMT:"))
                {
                    String[] incomingMsg = inAT[i].Split(seperator1, StringSplitOptions.None);
                    String[] msgInfo = incomingMsg[1].Split(seperator2, StringSplitOptions.None);

                    String pNum = "?";
                    if (msgInfo.Length > 7)
                    {
                        pNum = msgInfo[7].Replace("\"", "");
                    }
                    mCore.mSqlDb.addIncomingEntry(pNum, incomingMsg[2]);
                }

                if (inAT[i].Contains("+WEND: 25"))
                {
                    offHook = false;
                    modemReady = true;
                }

                if (inAT[i].Contains("+WIND: 10"))
                {
                    //??????
                }


            }    ///////////////inAT loop

        }




        /// Mock Modem: in case you don't have a real modem yet!
        private string MockWrite(string msg){
            string result = "";
            try
            {
                if (msg.StartsWith("AT+CNMI"))
                {
                    ParseModemResponse("OK");
                }
                else if (msg.StartsWith("ATE0"))
                {
                    ParseModemResponse("OK");
                }
                else if (msg.StartsWith("AT+CMGS"))
                {
                    // DataRecieved("OK");
                }
                else if (msg.EndsWith(Convert.ToString(CTRL_Z)))
                {
                    ParseModemResponse("+CMGS: 1");

                    Thread.Sleep(100);
                    ParseModemResponse("+CDS:2,1,\"8582431439\",129,\"02/05/17,10 :14 :17\",\"02/05/17,10 :14 :27\",32768");
                }
            }
            catch (Exception e)
            {
                doModemLog("Error: " + e.Message, 1);
                ParseModemResponse("ERROR");
            }
            return result;
        }

    }
}
