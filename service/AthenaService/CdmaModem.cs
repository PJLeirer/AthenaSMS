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

        //debug
        private bool isFakeModem = false; //arduino mock modem
        //private byte CTRL_W = (byte)0x17;
        //private byte SHIFT_OUT = (byte)0x0E;
        //private byte SHIFT_IN = (byte)0x0F;

        private char CTRL_Z = (char)0x1A;

        /*
        //stats report codes
        //
        //Network Problems
        int CDS_ADDRESS_VACANT = 0;// Address vacant
        int CDS_ADDRESS_TRANSLATION_FAILURE = 1;// Address translation failure
        int CDS_NETWORK_RESOURCE_SHORTAGE = 2;// Network resource shortage
        int CDS_NETWORK_FAILURE = 3;// Network failure
        int CDS_INVALID_TELESERVICE_ID = 4;// Invalid teleservice ID
        int CDS_OTHER_NETWORK_PROBLEM = 5;// Other network problem
        
        //Terminal Problems
        int CDS_NO_PAGE_RESPONSE = 32;// No page response
        int CDS_DESTINATION_BUSY = 33;// Destination busy
        int CDS_MN_ACKNOWLEDGE = 34;// No acknowledgment from transport layer
        int CDS_DESTINATION_RESOURCE_SHORTAGE = 35;// Destination resource shortage
        int CDS_DELIVERY_POSTPONED = 36;// SMS delivery postponed
        int CDS_DESTINATION_OUT_OF_SERVICE = 37;// Destination out of service
        int CDS_DESTINATION_NO_LONGER_AT_ADDRESS = 38;// Destination no longer at this address
        int CDS_OTHER_TERMINAL_PROBLEM = 39;// Other terminal problem

        //Radio Interface Problems
        int CDS_RADIO_INTERFACE_RESOURCE_SHORTAGE = 64;// Radio interface resource shortage
        int CDS_RADIO_INTERFACE_INCOMPATIBLE = 65;// Radio interface incompatible
        int CDS__OTHER_RADIO_INTERFACE_PROBLEM = 66;// Other radio interface problem

        //General problems (IS-41D)
        int CDS_UNEXPECTED_PARAMETER_SIZE = 96;// Unexpected parameter size
        int CDS_ORIGINATION_DENIED = 97;// SMS Origination denied
        int CDS_TERMINATION_DENIED = 98;// SMS Termination denied
        int CDS_SUPPLEMENTARY_SERVICE_NOT_SUPPORTED = 99;// Supplementary service not supported
        int CDS_SMS_NOT_SUPPORTED = 100;// SMS not supported
        int CDS_RESERVED1 = 101;// Reserved
        int CDS_MISSING_EXPECTED_PARAMETERS = 102;// Missing expected parameters
        int CDS_MISSING_MANDATORY_PARAMETERS = 103;// Missing mandatory parameters
        int CDS_UNRECOGNIZED_PARAMETER_VALUE1 = 104;// Unrecognized parameter value
        int CDS_UNRECOGNIZED_PARAMETER_VALUE2 = 105;// Unexpected parameter value
        int CDS_USER_DATA_SIZE_ERROR = 106;// User data size error
        //int CDS_NO_ACKNOWLEDGE_UNKOWN_ERROR = 107;// -255 No acknowledgement / Unknown error

        //General Codes
        int CDS_SMS_OK = 32768;// SMS OK. Message successfully delivered to base station
        int CDS_OUT_OF_RESOURCES = 32770;// Out of resources
        int CDS_MESSAGE_TOO_LARGE_FOR_ACCESS_CHANNEL = 32771;// Message too large for access channel
        int CDS_MESSAGE_TOO_LARGE_FOR_DEDICATED_CHANNEL = 32772;// Message too large for dedicated channel
        int CDS_NETOWRK_NOT_READY = 32773;// Network not ready
        int CDS_PHONE_NOT_READY = 32774;// Phone not ready
        int CDS_NOT_ALLOWED_IN_AMPS = 32775;// Not allowed in AMPS
        int CDS_CANNOT_SEND_BROADCAST = 32776;// Cannot send broadcast
        */


        private int BAUD = 115200;
        private String portName = null;
        public bool modemReady = false;
        public bool offHook = false;
        public SerialPort serialPort = null;

        public bool isReading = false;

        private String currentNumber = null;
        private String currentMessage = null;
        public int myNumber;

        public String[] currentSms = new String[2];
        public bool installedProperly = false;

        public StreamWriter mModemLog;

        String[] seperator0 = new String[] { "\r\n\r\n" };
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
                    mModemLog.WriteLine("<output>\n" + s + "\n</output>\n   @ " + ext + "\n\n");
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
                }
                else
                {
                    serialPort = new SerialPort(portName, BAUD, Parity.None, 8, StopBits.One);
                    serialPort.RtsEnable = true;
                    //serialPort.DtrEnable = true;
                    //serialPort.Handshake = Handshake.RequestToSend;
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
                        Program.mSqlDb.addOutgoingEntry(myNumber, cmgsInfo[1].Trim(), currentNumber, currentMessage);
                    }

                    if (inAT[i].Contains("ERROR"))
                    {
                        modemReady = true;
                        offHook = false;
                        Program.doEventLog("ERROR from modem on current text num: '" +
                                currentNumber + "' msg:" + currentMessage + " at " +
                                Program.mSqlDb.getTimeStamp(), 0);
                        Program.mSqlDb.failedOutgoing(currentNumber, currentMessage);
                    }

                    if (inAT[i].Contains("+CDS:"))
                    {
                        modemReady = true;
                        offHook = false;
                        Program.mSqlDb.updateOutgoingEntryReport(myNumber, inAT[i]);
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
                        offHook = false;
                        modemReady = true;
                    }

                    if (inAT[i].Contains("+WIND: 10"))
                    {
                        //??????
                    }


                }    ///////////////inAT loop

            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.Reader: " + e.Message + "\r\n" + e.StackTrace, 0);
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
            offHook = true;
            currentNumber = num;
            currentMessage = msg;
            try
            {

                doModemLog("AT+CMGS=\"" + num + "\"", 0);
                serialPort.Write("AT+CMGS=\"" + num + "\"\r");


                if (isFakeModem)
                {
                    Thread.Sleep(4000);
                }
                else
                {
                    Thread.Sleep(2000);
                }

                doModemLog(msg + CTRL_Z, 0);
                serialPort.Write(msg + CTRL_Z);

                //Thread.Sleep(1000);
                modemReady = true;
                offHook = false;
            }
            catch (Exception e)
            {
                Program.doEventLog("CdmaModem.sendSMS(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }

    }
}
