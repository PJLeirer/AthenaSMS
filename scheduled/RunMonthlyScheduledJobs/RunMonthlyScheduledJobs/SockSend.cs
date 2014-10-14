using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RunMonthlyScheduledJobs
{
    public class SockSend
    {
        public AutoResetEvent loginEvent;
        public AutoResetEvent msgEvent;
        public AutoResetEvent userListEvent;
        public AutoResetEvent groupsListEvent;
        public AutoResetEvent onlineListEvent;
        public AutoResetEvent contactsListEvent;
        public AutoResetEvent addUserEvent;
        public AutoResetEvent addContactEvent;
        public AutoResetEvent editContactEvent;
        public AutoResetEvent deleteContactEvent;
        public AutoResetEvent getContactInfoEvent;
        public AutoResetEvent deleteUserEvent;
        public AutoResetEvent changePassEvent;
        public AutoResetEvent changeLevelEvent;
        public AutoResetEvent singleTextEvent;
        public AutoResetEvent groupTextEvent;
        public AutoResetEvent textLogEvent;
        public AutoResetEvent failedLogEvent;
        public AutoResetEvent getModemsEvent;
        public AutoResetEvent getModemLogEvent;
        public AutoResetEvent getSysMsgModeEvent;
        public AutoResetEvent setSysMsgModeEvent;
        public AutoResetEvent getDayTotalsEvent;
        public AutoResetEvent getReportEvent;
        public AutoResetEvent kickConnEvent;
        public AutoResetEvent getJobInfoEvent;
        public AutoResetEvent getScheduledJobsEvent;
        public AutoResetEvent addGroupEvent;
        public AutoResetEvent deleteGroupEvent;
        public AutoResetEvent runSingleJobEvent;
        public AutoResetEvent runScheduledJobsEvent;
        public AutoResetEvent addScheduledJobEvent;
        public AutoResetEvent editScheduledJobEvent;
        public AutoResetEvent deleteScheduledJobEvent;




        public TcpClient client = null;
        public bool doRun = true;

        string xmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<AthenaObj>\n";
        string xmlEnd = "</AthenaObj>\n" + (char)0X00;

        StreamWriter mWriter;


        private Object myLock = new Object();



        public SockSend(TcpClient c)
        {
            client = c;
            mWriter = new StreamWriter(client.GetStream());
            loginEvent = new AutoResetEvent(false);
            msgEvent = new AutoResetEvent(false);
            userListEvent = new AutoResetEvent(false);
            groupsListEvent = new AutoResetEvent(false);
            onlineListEvent = new AutoResetEvent(false);
            contactsListEvent = new AutoResetEvent(false);
            addUserEvent = new AutoResetEvent(false);
            addContactEvent = new AutoResetEvent(false);
            editContactEvent = new AutoResetEvent(false);
            deleteContactEvent = new AutoResetEvent(false);
            getContactInfoEvent = new AutoResetEvent(false);
            deleteUserEvent = new AutoResetEvent(false);
            changePassEvent = new AutoResetEvent(false);
            changeLevelEvent = new AutoResetEvent(false);
            singleTextEvent = new AutoResetEvent(false);
            groupTextEvent = new AutoResetEvent(false);
            textLogEvent = new AutoResetEvent(false);
            failedLogEvent = new AutoResetEvent(false);
            getModemsEvent = new AutoResetEvent(false);
            getModemLogEvent = new AutoResetEvent(false);
            getSysMsgModeEvent = new AutoResetEvent(false);
            setSysMsgModeEvent = new AutoResetEvent(false);
            getDayTotalsEvent = new AutoResetEvent(false);
            getReportEvent = new AutoResetEvent(false);
            kickConnEvent = new AutoResetEvent(false);
            getJobInfoEvent = new AutoResetEvent(false);
            getScheduledJobsEvent = new AutoResetEvent(false);
            addGroupEvent = new AutoResetEvent(false);
            deleteGroupEvent = new AutoResetEvent(false);
            runSingleJobEvent = new AutoResetEvent(false);
            runScheduledJobsEvent = new AutoResetEvent(false);
            addScheduledJobEvent = new AutoResetEvent(false);
            editScheduledJobEvent = new AutoResetEvent(false);
            deleteScheduledJobEvent = new AutoResetEvent(false);
        }


        // close connection, only call from shutdown
        public void CloseConnection()
        {
            try
            {
                string xmlInfo = xmlStart;
                xmlInfo += "<txCloseConn>\n";
                xmlInfo += "<close>yes</close>\n";
                xmlInfo += "</txCloseConn>\n";
                xmlInfo += xmlEnd;
                lock (mWriter)
                {
                    mWriter.Write(xmlInfo);
                    mWriter.Flush();
                }
            }
            catch (Exception e)
            {
               // mMainWin.ErrorLogUpdater("SockMainSend.CloseConnection: " + e.Message + "\n\n" + e.StackTrace);
            }
            
        }


        public bool Login(String U, String P)
        {
            bool X = false;
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("sending login request");

                    //send xml

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txUserLogin>\n";
                    xmlInfo += "<user>" + U + "</user>\n";
                    xmlInfo += "<pass>" + P + "</pass>\n";
                    xmlInfo += "</txUserLogin>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    //mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    loginEvent.WaitOne(15000);
                    loginEvent.Reset();

                    Console.WriteLine("Checking user info");

                    if (Program.mSyncUser.isLoggedIn())
                    {
                        X = true;
                    }

                }
                catch (Exception e)
                {
                    //mMainWin.ErrorLogUpdater("SockMainSend.login: " + e.Message + "\n\n" + e.StackTrace);
                    X = false;
                }
            }
            return X;
        }




        public void RunScheduledJobs(int s)
        {
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("running scheduled jobs '" + s);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txRunScheduledJobs>";
                    xmlInfo += "<schedule>" + s + "</schedule>";
                    xmlInfo += "</txRunScheduledJobs>";
                    xmlInfo += xmlEnd;

                    // do log
                    //mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    runScheduledJobsEvent.WaitOne(15000);
                    runScheduledJobsEvent.Reset();

                }
                catch (Exception e)
                {
                    //mMainWin.ErrorLogUpdater("SockMainSend.RunDailyJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }




    }
}
