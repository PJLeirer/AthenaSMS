using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AthenaConsole
{
    public class SockMan
    {


        public SockRecv mSockRecv;
        public SockSend mSockSend;
        public SyncUser mSyncUser;

        MainWindow mMainWin;
        public TcpClient mClient;


        public bool addedUser = false;
        public bool deletedUser = false;
        public bool changedPass = false;
        public bool changedLevel = false;
        public bool sentGroupTxt = false;
        public bool sentSingleTxt = false;

        public AutoResetEvent loginEvent;


        public SockMan(MainWindow w)
        {
            mMainWin = w;


            loginEvent = new AutoResetEvent(false);


            new Thread(new ThreadStart(looper)).Start();
        }



        // NOT WORKING
        private void checkConnection()
        {
            /*
            while (MainWindow.isRunning)
            {
                if (!mClient.Client.Poll(500, SelectMode.SelectWrite))
                {
                    //MessageBox.Show("The Server has Disconnected", "Athena", MessageBoxButton.OK);
                    mMainWin.ShutDown();
                }
                Thread.Sleep(500);
            }
             */
        }

        private void looper()
        {
            using (TcpClient client = new TcpClient())
            {
                mClient = client;
                //
                try
                {
                    mClient.Connect(MainWindow.hostIP, MainWindow.hostPort);
                    if (mClient.Connected)
                    {
                        new Thread(new ThreadStart(checkConnection)).Start();

                        mSyncUser = new SyncUser();
                        mSockRecv = new SockRecv(mClient, mMainWin);
                        mSockSend = new SockSend(mClient, mMainWin);


                        mMainWin.ShowLogin();
                        loginEvent.WaitOne(30000);
                        loginEvent.Reset();

                        // run updates
                        if (mSyncUser.isLoggedIn())
                        {

                            mMainWin.Dispatcher.Invoke(
                                System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(
                                  delegate()
                                  {
                                      mMainWin.tAdminTab.BringIntoView();
                                  }
                              ));


                            while (MainWindow.isRunning)
                            {

                                //just keep thread alive to keep client connected

                                // check connection
                                //if (!mClient.Client.Connected)
                                //{
                                //    MessageBox.Show("The Server has Disconnected", "Athena", MessageBoxButton.OK);
                                //    mMainWin.ShutDown();
                                //}

                                Thread.Sleep(2000);
                            }

                        }
                        else
                        {
                            mMainWin.ShowLogin();
                            mMainWin.mLoginWin.lLoginErrorMessage.Content = "Failed to Login";
                        }

                    }
                }
                catch (SocketException e)
                {
                    mMainWin.ShowOptions("Unable to Connect to Athena", MainWindow.hostIP+"");
                }
            }
        }










    }
}
