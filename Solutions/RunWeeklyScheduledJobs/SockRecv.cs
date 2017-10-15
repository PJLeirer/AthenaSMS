using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace RunWeeklyScheduledJobs
{
    public class SockRecv
    {
        public TcpClient client = null;
        public bool doRun = true;
        private bool athena_obj;

        NetworkStream mStream;


        char endByt = (char)0X00;



        public SockRecv(TcpClient c)
        {
            client = c;
            mStream = client.GetStream();
            new Thread(looper).Start();
        }


        void looper()
        {
            while (doRun)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    while (mStream.DataAvailable)
                    {
                        char byt = (char)mStream.ReadByte();
                        if (byt == endByt)
                        {
                            break;
                        }
                        sb.Append(byt);
                    }




                    string xdata = sb.ToString();
                    if (xdata.Length > 0)
                    {

                        Console.WriteLine(xdata + "\r\n");

                        if (xdata.Length < 10)
                        {
                            //parse xdata
                        }
                        else
                        {

                            using (XmlReader mXmlReader = XmlReader.Create(new StringReader(xdata)))
                            {

                                athena_obj = false;
                                while (mXmlReader.Read())
                                {
                                    if (mXmlReader.IsStartElement())
                                    {
                                        if (mXmlReader.Name.Trim().Equals("AthenaObj"))
                                        {
                                            athena_obj = true;
                                        }

                                        if (athena_obj)
                                        {




                                            if (mXmlReader.Name.Trim().Equals("txLoginResult"))
                                            {
                                                Console.WriteLine("got txUserInfo");
                                                int userId = 0;
                                                string userName = null;
                                                int userLevel = 0;

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("id"))
                                                    {
                                                        mXmlReader.Read();
                                                        userId = Int32.Parse(mXmlReader.Value.Trim());
                                                        Console.WriteLine("userid: " + userId);
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                }
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("name"))
                                                    {
                                                        mXmlReader.Read();
                                                        userName = mXmlReader.Value.Trim().ToLower();
                                                        Console.WriteLine("username: " + userName);
                                                        mXmlReader.Read();// end tag
                                                    }
                                                }
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("level"))
                                                    {
                                                        mXmlReader.Read();
                                                        userLevel = Int32.Parse(mXmlReader.Value.Trim().ToLower());
                                                        Console.WriteLine("userlvl: " + userLevel);
                                                        //mXmlReader.Read();// end tag
                                                    }
                                                }

                                                

                                                if (userId > 0 && userName != null && userLevel > 0)
                                                {
                                                    Console.WriteLine("assigning info!");
                                                    Program.mSyncUser.assignUserInfo(userId, userName, userLevel);
                                                }


                                                Console.WriteLine("Releasing event!");


                                                Program.mSockMan.mSockSend.loginEvent.Set();


                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txGetScheduledJobsResult"))
                                            {
                                                bool didRunScheduledJobs = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("result"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didRunScheduledJobs = true;
                                                        }
                                                    }
                                                }

                                                Program.mSockMan.mSockSend.runScheduledJobsEvent.Set();

                                                string didRunJobsText;
                                                if (didRunScheduledJobs)
                                                {
                                                    didRunJobsText = "Running Jobs";
                                                }
                                                else
                                                {
                                                    didRunJobsText = "Failed to Run Jobs";
                                                }
                                                Console.WriteLine(didRunJobsText);
                                            }







                                        }
                                        // end of obj
                                    }
                                }
                            }

                        }
                    }





                }
                catch (Exception e)
                {
                    //mMainWin.ErrorLogUpdater("SockMainRecv.looper: " + e.Message + "\n\n" + e.StackTrace);
                    client.Close();
                    break;
                }
            }
        }//looper
    }
}
