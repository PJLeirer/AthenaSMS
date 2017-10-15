using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AthenaConsole
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

        MainWindow mMainWin;


        private Object myLock = new Object();



        public SockSend(TcpClient c, MainWindow w)
        {
            client = c;
            mMainWin = w;
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
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    loginEvent.WaitOne(15000);
                    loginEvent.Reset();

                    if (mMainWin.mSockMan.mSyncUser.isLoggedIn())
                    {
                        X = true;
                    }

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.login: " + e.Message + "\n\n" + e.StackTrace);
                    X = false;
                }
            }
            return X;
        }


        /*
        public void sendDailyHoldNotices() // leave for console
        {
            lock (myLock)
            {
                try
                {
                    //Console.WriteLine("running today's daily hold messages");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDailyHoldNotices>";
                    xmlInfo += "<dorun>yes</dorun>";
                    xmlInfo += "</txDailyHoldNotices>";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    msgEvent.WaitOne(15000);
                    msgEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.sendDailyHoldNotices: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }
        */




        public void AddAthenaUser(string u, string p, int l)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.addedUser = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddAthenaUser>\n";
                    xmlInfo += "<name>" + u + "</name>\n";
                    xmlInfo += "<pass>" + p + "</pass>\n";
                    xmlInfo += "<level>" + l + "</level>\n";
                    xmlInfo += "</txAddAthenaUser>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    addUserEvent.WaitOne(15000);
                    addUserEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaUser: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void DeleteAthenaUser(string u)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.deletedUser = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteAthenaUser>\n";
                    xmlInfo += "<name>" + u + "</name>\n";
                    xmlInfo += "</txDeleteAthenaUser>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    deleteUserEvent.WaitOne(15000);
                    deleteUserEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaUser: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void ChangeUserPass(string u, string p)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.changedPass = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txChangeUserPass>\n";
                    xmlInfo += "<name>" + u + "</name>\n";
                    xmlInfo += "<pass>" + p + "</pass>\n";
                    xmlInfo += "</txChangeUserPass>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    changePassEvent.WaitOne(15000);
                    changePassEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaUser: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void ChangeUserLevel(string u, int l)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.changedLevel = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txChangeUserLevel>\n";
                    xmlInfo += "<name>" + u + "</name>\n";
                    xmlInfo += "<level>" + l + "</level>\n";
                    xmlInfo += "</txChangeUserLevel>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    changeLevelEvent.WaitOne(15000);
                    changeLevelEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaUser: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void GetAllUsers()
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetUserList>\n";
                    xmlInfo += "<users>yes</users>\n";
                    xmlInfo += "</txGetUserList>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    userListEvent.WaitOne(15000);
                    userListEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetAllUsers: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void GetOnlineUsers()
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetOnlineList>\n";
                    xmlInfo += "<users>yes</users>\n";
                    xmlInfo += "</txGetOnlineList>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    onlineListEvent.WaitOne(15000);
                    onlineListEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetOnlineUsers: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }




        public void GetSysMsgMode()
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetSysMsgMode>\n";
                    xmlInfo += "<mode>yes</mode>\n";
                    xmlInfo += "</txGetSysMsgMode>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getSysMsgModeEvent.WaitOne(15000);
                    getSysMsgModeEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetSysMsgMode: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void SetSysMsgMode(int m)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txSetSysMsgMode>\n";
                    xmlInfo += "<mode>" + m + "</mode>\n";
                    xmlInfo += "</txSetSysMsgMode>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    setSysMsgModeEvent.WaitOne(15000);
                    setSysMsgModeEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.SetSysMsgMode: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }




        public void AddGroup(string name)
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddGroup>\n";
                    xmlInfo += "<group>" + name + "</group>\n";
                    xmlInfo += "</txAddGroup>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    addGroupEvent.WaitOne(15000);
                    addGroupEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.AddGroup: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void DeleteGroup(string name)
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteGroup>\n";
                    xmlInfo += "<group>" + name + "</group>\n";
                    xmlInfo += "</txDeleteGroup>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    deleteGroupEvent.WaitOne(15000);
                    deleteGroupEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.DeleteGroup: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void GetAllGroups()
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetGroupList>\n";
                    xmlInfo += "<groups>yes</groups>\n";
                    xmlInfo += "</txGetGroupList>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    groupsListEvent.WaitOne(15000);
                    groupsListEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetAllUsers: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }




        public void SendSingleText(string num, string msg)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txSngTxt>\n";
                    xmlInfo += "<number>" + num + "</number>\n";
                    xmlInfo += "<stxt>" + msg + "</stxt>\n";
                    xmlInfo += "</txSngTxt>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    singleTextEvent.WaitOne(15000);
                    singleTextEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.SendSingleText: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void SendGroupText(string grp, string msg)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGrpTxt>\n";
                    xmlInfo += "<group>" + grp + "</group>\n";
                    xmlInfo += "<gtxt>" + msg + "</gtxt>\n";
                    xmlInfo += "</txGrpTxt>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    groupTextEvent.WaitOne(15000);
                    groupTextEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.SendGroupText: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        




        public void AddAthenaContact(string n, string p, string g)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.addedUser = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddAthenaContact>\n";
                    xmlInfo += "<name>" + n + "</name>\n";
                    xmlInfo += "<phone>" + p + "</phone>\n";
                    xmlInfo += "<group>" + g + "</group>\n";
                    xmlInfo += "</txAddAthenaContact>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    addContactEvent.WaitOne(15000);
                    addContactEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaUser: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void EditAthenaContact(string n, string col, string val)
        {
            lock (myLock)
            {
                try
                {
                    mMainWin.mSockMan.addedUser = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txEditAthenaContact>\n";
                    xmlInfo += "<name>" + n + "</name>\n";
                    xmlInfo += "<col>" + col + "</col>\n";
                    xmlInfo += "<val>" + val + "</val>\n";
                    xmlInfo += "</txEditAthenaContact>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    editContactEvent.WaitOne(15000);
                    editContactEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.addAthenaContact: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void DeleteAthenaContact(string n)
        {
            lock (myLock)
            {
                try
                {
                    //mMainWin.mSockMan.addedUser = false;
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteAthenaContact>\n";
                    xmlInfo += "<name>" + n + "</name>\n";
                    xmlInfo += "</txDeleteAthenaContact>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    deleteContactEvent.WaitOne(15000);
                    deleteContactEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.deleteAthenaContact: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }






        public void GetContacts()
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetContactsList>\n";
                    xmlInfo += "<contacts>yes</contacts>\n";
                    xmlInfo += "</txGetContactsList>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    contactsListEvent.WaitOne(15000);
                    contactsListEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetContacts: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }






        public void GetContactInfo(string n)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetContactInfo>\n";
                    xmlInfo += "<name>" + n + "</name>\n";
                    xmlInfo += "</txGetContactInfo>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getContactInfoEvent.WaitOne(15000);
                    getContactInfoEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetContactInfo: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        


        


        public void GetTextLog(string f, string d)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetLog>\n";
                    xmlInfo += "<filter>" + f + "</filter>\n";
                    xmlInfo += "<direction>" + d + "</direction>\n";
                    xmlInfo += "</txGetLog>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    textLogEvent.WaitOne(15000);
                    textLogEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetTextLog: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }


        public void GetFailedLog()
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetFailed>\n";
                    xmlInfo += "<failed>yes</failed>\n";
                    xmlInfo += "</txGetFailed>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    failedLogEvent.WaitOne(15000);
                    failedLogEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetFailedLog: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }








        public void GetModems()
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetModems>\n";
                    xmlInfo += "<modems>yes</modems>\n";
                    xmlInfo += "</txGetModems>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getModemsEvent.WaitOne(15000);
                    getModemsEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetModems: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }





        //remove
        public void GetModemLog(int m)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetModemLog>\n";
                    xmlInfo += "<modem>" + m + "</modem>\n";
                    xmlInfo += "</txGetModemLog>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getModemLogEvent.WaitOne(15000);
                    getModemLogEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetModemLog: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }






        public void GetDayTotals(string sent)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetDayTotals>\n";
                    xmlInfo += "<day>" + sent + "</day>\n";
                    xmlInfo += "</txGetDayTotals>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getDayTotalsEvent.WaitOne(15000);
                    getDayTotalsEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetDayTotals: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }





        public void GetReport(int typ, int snd)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetReport>\n";
                    xmlInfo += "<type>" + typ + "</type>\n";
                    xmlInfo += "<send>" + snd + "</send>\n";
                    xmlInfo += "</txGetReport>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getReportEvent.WaitOne(15000);
                    getReportEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetReport: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
            
        }




        public void GetJobInfo(string name)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetJobInfo>\n";
                    xmlInfo += "<name>" + name + "</name>\n";
                    xmlInfo += "</txGetJobInfo>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getJobInfoEvent.WaitOne(15000);
                    getJobInfoEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetJobInfo: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }




        public void RunSingleJob(string name)
        {
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("running job '" + name);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txRunSingleJob>";
                    xmlInfo += "<name>" + name + "</name>";
                    xmlInfo += "</txRunSingleJob>";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    runSingleJobEvent.WaitOne(15000);
                    runSingleJobEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.RunDailyJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void GetScheduledJobs(int s)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetScheduledJobs>\n";
                    xmlInfo += "<schedule>" + s + "</schedule>\n";
                    xmlInfo += "</txGetScheduledJobs>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    getScheduledJobsEvent.WaitOne(15000);
                    getScheduledJobsEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.GetScheduledJobs: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
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
                    mMainWin.SocketOutUpdater(xmlInfo);

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
                    mMainWin.ErrorLogUpdater("SockMainSend.RunDailyJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }





        public void AddScheduledJob(int schedule, string name, string location, string file)
        {
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("adding job '" + name);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddScheduledJob>\n";
                    xmlInfo += "<schedule>" + schedule + "</schedule>\n";
                    xmlInfo += "<name>" + name + "</name>\n";
                    xmlInfo += "<location>" + location + "</location>\n";
                    xmlInfo += "<file>" + file + "</file>\n";
                    xmlInfo += "</txAddScheduledJob>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    addScheduledJobEvent.WaitOne(15000);
                    addScheduledJobEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.AddScheduledJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void EditScheduledJob(int schedule, string name, string location, string file)
        {
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("editing job '" + name);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txEditScheduledJob>\n";
                    xmlInfo += "<schedule>" + schedule + "</schedule>\n";
                    xmlInfo += "<name>" + name + "</name>\n";
                    xmlInfo += "<location>" + location + "</location>\n";
                    xmlInfo += "<file>" + file + "</file>\n";
                    xmlInfo += "</txEditScheduledJob>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    editScheduledJobEvent.WaitOne(15000);
                    editScheduledJobEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.EditScheduledJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



        public void DeleteScheduledJob(string name, int schedule)
        {
            lock (myLock)
            {
                try
                {
                    Console.WriteLine("deleting job '" + name);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteScheduledJob>\n";
                    xmlInfo += "<name>" + name + "</name>\n";
                    xmlInfo += "<schedule>" + schedule + "</schedule>\n";
                    xmlInfo += "</txDeleteScheduledJob>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    deleteScheduledJobEvent.WaitOne(15000);
                    deleteScheduledJobEvent.Reset();

                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.EditScheduledJob: " + e.Message + "\n\n" + e.StackTrace);
                }
            }
        }



         
       



        public void KickConnection(int c)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txKickConn>\n";
                    xmlInfo += "<conn>" + c + "</conn>\n";
                    xmlInfo += "</txKickConn>\n";
                    xmlInfo += xmlEnd;

                    // do log
                    mMainWin.SocketOutUpdater(xmlInfo);

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                    kickConnEvent.WaitOne(15000);
                    kickConnEvent.Reset();
                }
                catch (Exception e)
                {
                    mMainWin.ErrorLogUpdater("SockMainSend.KickConnection: " + e.Message + "\n\n" + e.StackTrace);
                }
            }

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
                mMainWin.ErrorLogUpdater("SockMainSend.CloseConnection: " + e.Message + "\n\n" + e.StackTrace);
            }

        }












    }
}
