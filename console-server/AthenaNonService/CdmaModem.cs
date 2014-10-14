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

namespace AthenaService
{
    class CdmaModem
    {

        // a mock modem can be programmed with a microcontroller, however without flow control, data can get lost easily
        // 
        private bool isFakeModem = false; //set true for mock modem settings

        private char CTRL_Z = (char)0x1A;

        // check your modem's init AT Command options, any changes must be the exact settings. these option setting are crucial.
        // I will add more info on these settings from the manual for MTCBA-C1-U-N3
        private string initCommand = "AT+CNMI=2,2,0,1,0";

        private int BAUD = 115200;
        private String portName = null;
        public bool modemReady = false;
        public SerialPort serialPort = null;

        public bool isReading = false;

        private String currentNumber = null;
        private String currentMessage = null;
        public int myNumber;

        public String[] currentSms = new String[2];
        public bool installedProperly = false;

        public StreamWriter mModemLog;

        //String[] seperator0 = new String[] { "\r\n\r\n" };
        String[] seperator1 = new String[] { "\r\n" };
        String[] seperator2 = new String[] { "," };
        String[] seperator3 = new String[] { " " };
        String[] seperator4 = new String[] { ":" };


        public CdmaModem(String port, int num)
        {
            portName = port;
            myNumber = num;

            initModem();
        }

        private void doModemLog(String s, int n) // 1=in, 0=out
        {
            try
            {
                DateTime myTime = DateTime.Now;
                string fn = myTime.ToString("MM-dd-yyyy");
                string ext = myTime.ToString("HH:mm:ss:ffff");
                mModemLog = new StreamWriter(Program.AthenaDir + @"log\modem" + myNumber + "_" + fn, true);
                if (n > 0)
                {
                    mModemLog.WriteLine("<input>\n" + s + "\n</input>" + ext + "\n\n");
                }
                else
                {
                    mModemLog.WriteLine("<output>\n" + s + "\n</output>\n   @" + ext + "\n\n");
                }
                mModemLog.Close();
            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.doModemLog: " + e.Message + "\r\n" + e.StackTrace, 1);
            }
            finally
            {
                if (mModemLog != null)
                {
                    mModemLog.Close();
                }
            }
        }

        private void initModem()
        {
            try
            {
                if (isFakeModem)
                {
                    serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                    serialPort.ReadTimeout = 8000;
                    serialPort.WriteTimeout = 8000;
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;
                    serialPort.Handshake = Handshake.None;
                }
                else
                {
                    serialPort = new SerialPort(portName, BAUD, Parity.None, 8, StopBits.One);
                    serialPort.RtsEnable = true;
                    serialPort.DtrEnable = true;
                    serialPort.Handshake = Handshake.RequestToSend;
                    serialPort.ReadTimeout = 500;
                    serialPort.WriteTimeout = 500;
                }

                serialPort.Open();

                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

                //DateTime time = DateTime.Now;
                //string fn = time.ToString("MM-dd-yyyy");
                //mModemLog = new StreamWriter(Program.AthenaDir + @"log\modem" + myNumber + "_" + fn, true);



                Thread.Sleep(1500);

                doModemLog("ATE0", 0);
                serialPort.Write("ATE0\r");

                Thread.Sleep(900);

                doModemLog("AT+CNMI=2,2,0,1,0", 0);
                serialPort.Write("AT+CNMI=2,2,0,1,0\r");

                installedProperly = true;
                modemReady = true;

            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.initModem: " + e.Message + "\r\n" + e.StackTrace, 0);
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


                doModemLog(input, 1);




                // parse
                String[] inAT = input.Split(new String[] { "\r\n\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < inAT.Length; i++)
                {

                    if (inAT[i].Contains("+WANS"))
                    {
                        modemReady = false;
                    }

                    if (inAT[i].Contains("+CMGS"))
                    {
                        //run to make sure it's splitting right
                        String[] cmgsInfo = inAT[i].Split(seperator4, StringSplitOptions.None);
                        Program.mSqlDb.addOutgoingEntry(myNumber, cmgsInfo[1].Trim(), currentNumber, currentMessage);
                    }

                    if (inAT[i].Contains("ERROR"))
                    {
                        string entry = "ERROR! modem" + myNumber + ": '" + currentNumber + "' msg:" + currentMessage;
                        Program.doEventLog(entry, 0);
                        Program.mSqlDb.addErrorLogEntry(entry);
                        modemReady = true;
                    }

                    if (inAT[i].Contains("+CDS:"))
                    {
                        Program.mSqlDb.updateOutgoingEntryReport(myNumber, inAT[i]);
                        modemReady = true;
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
                        Program.mSqlDb.addIncomingEntry(pNum, incomingMsg[2]);
                    }

                    if (inAT[i].Contains("+WEND: 25"))
                    {
                        //modemReady = true;
                    }

                    if (inAT[i].Contains("+WIND: 10"))
                    {
                        //??????
                    }


                }    ///////////////inAT loop

            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.DataRecieved: " + e.Message + "\r\n" + e.StackTrace, 0);
            }


        }


        public void ShutDown()
        {
            isReading = false;
            serialPort.Close();
        }


        public void sendSMS(String num, String msg)
        {
            modemReady = false;
            currentNumber = num;
            currentMessage = msg;
            try
            {

                doModemLog("AT+CMGS=\"" + num + "\"", 0);
                serialPort.Write("AT+CMGS=\"" + num + "\"\r");


                if (isFakeModem)
                {
                    Thread.Sleep(900);
                }
                else
                {
                    Thread.Sleep(600);
                }

                doModemLog(msg + CTRL_Z, 0);
                serialPort.Write(msg + CTRL_Z);

                Thread.Sleep(200);
                //modemReady = true; // wait for report to release modem
            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.sendSMS(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }

    }
}
