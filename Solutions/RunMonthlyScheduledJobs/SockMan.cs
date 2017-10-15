using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RunMonthlyScheduledJobs
{
    public class SockMan
    {


        public SockRecv mSockRecv;
        public SockSend mSockSend;

        public TcpClient mClient;


        public bool addedUser = false;
        public bool deletedUser = false;
        public bool changedPass = false;
        public bool changedLevel = false;
        public bool sentGroupTxt = false;
        public bool sentSingleTxt = false;


        public SockMan()
        {
            new Thread(new ThreadStart(
                delegate()
                {
                    using (TcpClient client = new TcpClient())
                    {
                        mClient = client;
                        //
                        try
                        {
                            mClient.Connect(Program.hostIP, Program.hostPort);
                            if (mClient.Connected)
                            {


                                mSockRecv = new SockRecv(mClient);
                                mSockSend = new SockSend(mClient);


                                //mMainWin.ShowLogin();

                                //login


                                Console.WriteLine("Logging in!");

                                // run updates
                                if (mSockSend.Login(Program.ClientUserName, Program.ClientUserPass))
                                {
                                    Console.WriteLine("Running Monthly Jobs");
                                    mSockSend.RunScheduledJobs(3);

                                }
                                else
                                {
                                    Console.WriteLine("Failed to Login");
                                }

                                //disconnect
                                mSockSend.CloseConnection();

                            }
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine("Unable to Connect to Athena '" + Program.hostIP + "'");
                        }
                    }
                }
              )).Start();
            


        }









    }
}
