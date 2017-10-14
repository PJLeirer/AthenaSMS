using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AthenaCore
{
    class SockReciever
    {

        public SockSender mSender;

        public bool doRun = true;

        public bool loggedIn = false;
        public int mUserId = 0;
        public String mUserName = "nobody";
        public int mUserLevel = 0;

        int sGroup;
        String sGxt;

        bool athena_obj;

        public TcpClient mClient;

        NetworkStream mStream;

        public bool deadClient = false;

        public Core mCore;


        public SockReciever(Core core,TcpClient c)
        {
            mCore = core;
            mClient = c;
            mStream = mClient.GetStream();

            mSender = new SockSender(core, mClient);

            new Thread(looper).Start();

        }

        private void looper()
        {
            while (doRun)
            {
                try
                {

                    //Console.WriteLine("Getting Data from stream");

                    int allBytes;

                    StringBuilder builder = new StringBuilder();

                    do
                    {

                        byte[] buff = new byte[1024];

                        allBytes = mStream.Read(buff, 0, buff.Length);
                        if (allBytes < 1)
                        {
                            builder.Clear();
                            doRun = false;
                            Console.WriteLine(mUserName + " disconnected");
                            mClient.Client.Shutdown(SocketShutdown.Both);
                            mClient.Client.Disconnect(true);
                            deadClient = true;
                            ///mClient.Close();
                            break;
                        }

                        for (int i = 0; i < buff.Length; i++)
                        {
                            if (buff[i] != 0x00)
                            {
                                builder.Append((char)buff[i]);
                            }
                        }

                    }
                    while (mStream.DataAvailable);


                    String xdata = builder.ToString();
                    if (xdata.Length > 0)
                    {


                        Console.WriteLine("procesing string.\r\n" + xdata);
                        Thread.Sleep(2000);


                        using (XmlReader mXmlReader = XmlReader.Create(new StringReader(xdata.Trim())))
                        {
                            athena_obj = false;
                            while (mXmlReader.Read())
                            {

                                //debug
                                //if (Program._debug) { Console.WriteLine("Got data '" + mXmlReader.Name + "'"); }


                                if (mXmlReader.IsStartElement())
                                {
                                    if (mXmlReader.Name.Trim().Equals("AthenaObj"))
                                    {
                                        athena_obj = true;
                                    }


                                    if (athena_obj)
                                    {

                                        switch (mXmlReader.Name.Trim())
                                        {

                                            case "txUserLogin":

                                                string userName = "";
                                                string userPass = "";

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("user"))
                                                    {
                                                        mXmlReader.Read();
                                                        userName = mXmlReader.Value.Trim();
                                                        //if (Program._debug) { Console.WriteLine("username: " + userName); }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("pass"))
                                                    {
                                                        mXmlReader.Read();
                                                        userPass = mXmlReader.Value.Trim().ToLower();
                                                        //if (Program._debug) { Console.WriteLine("userpass: " + userPass); }
                                                        mXmlReader.Read();// end tag
                                                    }
                                                }

                                                if (userName.Length > 0 && userPass.Length > 0)
                                                {
                                                    string[] info = mCore.mSqlDb.getUserInfo(userName, userPass);
                                                    if (info != null)
                                                    {
                                                        if (info[0] != null)
                                                        {
                                                            mUserId = Int32.Parse(info[0]);
                                                        }
                                                        if (info[1] != null)
                                                        {
                                                            mUserName = info[1];
                                                        }
                                                        if (info[2] != null)
                                                        {
                                                            mUserLevel = Int32.Parse(info[2]);
                                                        }
                                                    }
                                                    mSender.sendUserInfo(mUserId, mUserName, mUserLevel);
                                                }
                                                else
                                                {
                                                    //if (Program._debug) { Console.WriteLine("username or password is not long enough"); }
                                                }

                                                break;


                                            case "txFunx":


                                                break;


                                            case "txGrpTxt":

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("group"))
                                                    {
                                                        mXmlReader.Read();
                                                        sGroup = Int32.Parse(mXmlReader.Value);
                                                        //if (Program._debug) { Console.WriteLine("group: " + sGroup); }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("gtxt"))
                                                    {
                                                        mXmlReader.Read();
                                                        sGxt = mXmlReader.Value.Trim();
                                                        //if (Program._debug) { Console.WriteLine("gtxt: " + sGxt); }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }

                                                if (sGxt != null)
                                                {
                                                    Resources.mGroups.textGroup(sGroup, sGxt);
                                                    //mSender.
                                                }
                                                else
                                                {
                                                    // if (Program._debug) { Console.WriteLine("text is not long enough"); }
                                                }

                                                break;


                                            case "txSngTxt":


                                                break;


                                            case "txGetLog":

                                                string filter = "";
                                                string direction = "";

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("filter"))
                                                    {
                                                        mXmlReader.Read();
                                                        filter = mXmlReader.Value.Trim();
                                                        //if (Program._debug) { Console.WriteLine("username: " + userName); }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("direction"))
                                                    {
                                                        mXmlReader.Read();
                                                        direction = mXmlReader.Value.Trim();
                                                        //if (Program._debug) { Console.WriteLine("username: " + userName); }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }

                                                ArrayList log;
                                                int todaysIncoming = 0;
                                                int todaysOutgoing = 0;

                                                if (direction.Equals("in"))
                                                {
                                                    log = mCore.mSqlDb.getIncomingLog(filter);
                                                }
                                                else
                                                {
                                                    log = mCore.mSqlDb.getOutgoingLog(filter);
                                                }
                                                int[] count = mCore.mSqlDb.getTodaysCount();
                                                todaysIncoming = count[0];
                                                todaysOutgoing = count[1];
                                                mSender.sendTxtLog(log, todaysIncoming, todaysOutgoing);

                                                break;

                                            case "txGetGroupList":

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("groups"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            mSender.sendGroupList();
                                                        }
                                                    }
                                                    mXmlReader.Read(); //end tag

                                                }

                                                break;

                                            case "txGetUserList":

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("users"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            mSender.sendUserList();
                                                        }
                                                    }
                                                    mXmlReader.Read(); //end tag

                                                }

                                                break;


                                            case "txDailyHoldNotices":

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("dorun"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            if (mCore.runDailyJob())
                                                            {
                                                                mSender.sendData("success", "yes");
                                                            }
                                                            else
                                                            {
                                                                mSender.sendData("success", "no");
                                                            }
                                                        }
                                                    }
                                                    mXmlReader.Read(); //end tag

                                                }

                                                break;



                                        }


                                    }

                                }
                            }
                        }
                    }




                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockReciever.looper " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }

    }
}
