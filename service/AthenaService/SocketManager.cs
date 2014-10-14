using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaService
{
    class SocketManager
    {

        public int sockPort = 11420;
        public TcpListener sockSrv = null;
        public ArrayList mRecievers;
        public bool doRun = false;
        public String amReady = "Not Running!";



        public SocketManager()
        {
            mRecievers = new ArrayList();
            new Thread(looper).Start();
            new Thread(reclaim).Start();
        }


        // still seems to have some bugs when releasing. does not release at all
        private void reclaim()
        {
            while (doRun)
            {
                try
                {

                    for (int x = 0; x < mRecievers.Count; x++)
                    {
                        SockReciever client = (SockReciever)mRecievers[x];
                        if (client.deadClient || !client.mClient.Client.Connected || !client.mClient.Connected)
                        {
                            mRecievers.Remove(x);
                            x--;
                        }
                    }

                    Thread.Sleep(200);

                }
                catch (Exception e)
                {
                    Program.doEventLog("SocketManager.reclaim: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }

        private void looper()
        {
            try
            {
                sockSrv = new TcpListener(IPAddress.Any, sockPort);
                sockSrv.Start();
                doRun = true;
                while (doRun)
                {
                    mRecievers.Add(new SockReciever(sockSrv.AcceptTcpClient()));
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SocketManager.looper: " + e.Message + "\r\n" + e.StackTrace, 2);
            }
        }

        public ArrayList WhosOnline()
        {
            ArrayList userList = new ArrayList();
            int i = 0;
            foreach (SockReciever sr in mRecievers)
            {
                userList.Add("User: " + sr.mUserName + " - Connection: " + i);
                i++;
            }
            return userList;
        }

        public void ShutDown()
        {
            doRun = false;
            for (int i = 0; i < mRecievers.Count; i++)
            {
                if (mRecievers[i] != null)
                {
                    SockReciever r = (SockReciever)mRecievers[i];
                    r.doRun = false;
                    try
                    {
                        r.mClient.Client.Shutdown(SocketShutdown.Both);
                        r.mClient.Client.Disconnect(true);
                        r.mClient.Close();

                    }
                    catch (Exception e)
                    {
                        Program.doEventLog("SocketManager.looper: " + e.Message + "\r\n" + e.StackTrace, 1);
                    }
                }
            }

            //sockSrv.Server.Shutdown(SocketShutdown.Both);
            //sockSrv.Server.Dispose();
            sockSrv.Stop();
            mRecievers = null;


        }

    }

}
