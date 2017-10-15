using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class SocketManager
    {

        public int sockPort = 11420;
        public TcpListener sockSrv = null;
        public ArrayList mRecievers;
        public bool doRun = false;
        public String amReady = "Not Running!";

        public Core mCore;

        public SocketManager(Core core)
        {
            mCore = core;
            doRun = true;
            mRecievers = new ArrayList();
            new Thread(looper).Start();
            new Thread(reclaim).Start();
        }


        private void reclaim()
        {
            while (doRun)
            {
                try
                {
                    lock (mRecievers.SyncRoot)
                    {
                        for (int i = 0; i < mRecievers.Count; i++)
                        {
                            SockReciever reciever = (SockReciever)mRecievers[i];
                            //if (reciever.mClient.Client.Connected && reciever.mClient.GetStream().CanWrite)
                            //{
                            //reciever.mClient.GetStream().WriteByte((byte)0);
                            //}
                            //Console.WriteLine("reciever: " + reciever.mClient.Client.ToString());
                            if (reciever.deadClient || !reciever.mClient.Client.Connected)
                            {
                                Console.WriteLine("Disconnecting dead client");
                                //client.mClient.GetStream().Close();
                                reciever.mClient.Close();
                                mRecievers.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SocketManager.reclaim: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }

        public bool KickConnection(int n)
        {
            bool X = false;
            try
            {

                SockReciever rec = (SockReciever)mRecievers[n];
                rec.doRun = false;
                rec.mClient.GetStream().Close();
                rec = null;
                lock (mRecievers.SyncRoot)
                {
                    mRecievers.RemoveAt(n);
                }
                X = true;
            }
            catch (Exception e)
            {
                mCore.doEventLog("SocketManager.KickConnection: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return X;
        }

        private void looper()
        {
            try
            {
                sockSrv = new TcpListener(IPAddress.Any, sockPort);
                sockSrv.Start();
                while (doRun)
                {
                    SockReciever reciever = new SockReciever(mCore, mRecievers, sockSrv.AcceptTcpClient());
                    lock (mRecievers.SyncRoot)
                    {
                        mRecievers.Add(reciever);
                    }
                }
            }
            catch (Exception e)
            {
                mCore.doEventLog("SocketManager.looper: " + e.Message + "\r\n" + e.StackTrace, 2);
            }
        }

        public ArrayList WhosOnline()
        {
            ArrayList userList = new ArrayList();
            try
            {
                int i = 0;
                foreach (SockReciever sr in mRecievers)
                {
                    userList.Add(sr.mUserName + "  -  conn:" + i);
                    i++;
                }
            }
            catch (Exception e)
            {
                mCore.doEventLog("SocketManager.WhosOnline: " + e.Message + "\r\n" + e.StackTrace, 2);
            }
            return userList;
        }

        public void ShutDown()
        {
            doRun = false;
            try
            {
                lock (mRecievers.SyncRoot)
                {
                    for (int i = 0; i < mRecievers.Count; i++)
                    {
                        if (mRecievers[i] != null)
                        {
                            SockReciever r = (SockReciever)mRecievers[i];
                            r.doRun = false;
                            r.mClient.Client.Shutdown(SocketShutdown.Both);
                            r.mClient.Client.Disconnect(true);
                            r.mClient.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mCore.doEventLog("SocketManager.looper: " + e.Message + "\r\n" + e.StackTrace, 1);
            }

            //sockSrv.Server.Shutdown(SocketShutdown.Both);
            //sockSrv.Server.Dispose();
            sockSrv.Stop();
            mRecievers = null;


        }

    }

}
