/*
 * SockSender
 * 
 * sub class of SockReciever for returning data on recieve, also used with broadcasting messages to connected clients
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AthenaCore
{
    public class SockSender
    {

        StreamWriter mWriter;

        public Core mCore;

        string xmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<AthenaObj>\n";
        string xmlEnd = "</AthenaObj>\n" + (char)0X00;

        private Object myLock = new Object();


        public SockSender(Core core, TcpClient c)
        {
            mCore = core;
            mWriter = new StreamWriter(c.GetStream());
        }



        public void sendLoginInfo(int userId, String userName, int userLevel)
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txLoginResult>\n";
                    xmlInfo += "<id>" + userId + "</id>\n";
                    xmlInfo += "<name>" + userName + "</name>\n";
                    xmlInfo += "<level>" + userLevel + "</level>\n";
                    xmlInfo += "</txLoginResult>\n";
                    xmlInfo += xmlEnd;

                    Console.WriteLine("sending... \n" + xmlInfo);
                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendUserInfo(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }





        public void SendAddUserResult(bool b)
        {
            lock (myLock)
            {
                try
                {


                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddUserResult>\n";
                    if (b)
                    {
                        xmlInfo += "<added>yes</added>\n";
                    }
                    else
                    {
                        xmlInfo += "<added>no</added>\n";
                    }
                    xmlInfo += "</txAddUserResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendAddUserResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }







        public void SendDeleteUserResult(bool b)
        {
            lock (myLock)
            {
                try
                {


                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteUserResult>\n";
                    if (b)
                    {
                        xmlInfo += "<deleted>yes</deleted>\n";
                    }
                    else
                    {
                        xmlInfo += "<deleted>no</deleted>\n";
                    }
                    xmlInfo += "</txDeleteUserResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendDeleteUserResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void SendChangeUserPassResult(bool b)
        {
            lock (myLock)
            {
                try
                {


                    string xmlInfo = xmlStart;
                    xmlInfo += "<txChangeUserPassResult>\n";
                    if (b)
                    {
                        xmlInfo += "<changed>yes</changed>\n";
                    }
                    else
                    {
                        xmlInfo += "<changed>no</changed>\n";
                    }
                    xmlInfo += "</txChangeUserPassResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendChangeUserPassResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void SendChangeUserLevelResult(bool b)
        {
            lock (myLock)
            {
                try
                {


                    string xmlInfo = xmlStart;
                    xmlInfo += "<txChangeUserLevelResult>\n";
                    if (b)
                    {
                        xmlInfo += "<changed>yes</changed>\n";
                    }
                    else
                    {
                        xmlInfo += "<changed>no</changed>\n";
                    }
                    xmlInfo += "</txChangeUserLevelResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendDeleteLevelResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }











        /* old

        public void sendData(string tag, string val)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending data " + tag + ":" + val);

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txData>\n";
                    xmlInfo += "<" + tag + ">" + val + "</" + tag + ">\n";
                    xmlInfo += "</txData>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendData(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }

        */





        public void sendUserList()
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending userList ");

                    ArrayList users = mCore.mSqlDb.getUserList();

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txUserList>\n";
                    for (int i = 0; i < users.Count; i++)
                    {
                        xmlInfo += "<user>" + users[i] + "</user>\n";
                    }
                    xmlInfo += "</txUserList>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void sendOnlineList(ArrayList users)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending onlineList ");



                    string xmlInfo = xmlStart;
                    xmlInfo += "<txOnlineList>\n";
                    for (int i = 0; i < users.Count; i++)
                    {
                        xmlInfo += "<user>" + users[i] + "</user>\n";
                    }
                    xmlInfo += "</txOnlineList>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void SendGetMsgModeResult(int n)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetMsgModeResult>\n";
                    xmlInfo += "<mode>" + n + "</mode>\n";
                    xmlInfo += "</txGetMsgModeResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendGetMsgModeResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void SendSetMsgModeResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txSetMsgModeResult>\n";
                    if (b)
                    {
                        xmlInfo += "<mode>yes</mode>\n";
                    }
                    else
                    {
                        xmlInfo += "<mode>no</mode>\n";
                    }
                    xmlInfo += "</txSetMsgModeResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendSetMsgModeResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }










        public void SendAddGroupResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddGroupResult>\n";
                    if (b)
                    {
                        xmlInfo += "<added>yes</added>\n";
                    }
                    else
                    {
                        xmlInfo += "<added>no</added>\n";
                    }
                    xmlInfo += "</txAddGroupResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendAddGroupResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void SendDeleteGroupResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteGroupResult>\n";
                    if (b)
                    {
                        xmlInfo += "<deleted>yes</deleted>\n";
                    }
                    else
                    {
                        xmlInfo += "<deleted>no</deleted>\n";
                    }
                    xmlInfo += "</txDeleteGroupResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendDeleteGroupResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void sendGroupList()
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending groupList ");

                    ArrayList groups = mCore.mSqlDb.getGroupList();

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGroupList>\n";
                    for (int i = 0; i < groups.Count; i++)
                    {
                        xmlInfo += "<group>" + groups[i] + "</group>\n";
                    }
                    xmlInfo += "</txGroupList>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendGroupList(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }







        public void SendGrpTxtResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGrpTxtResult>\n";
                    xmlInfo += "<sent>" + result + "</sent>\n";
                    xmlInfo += "</txGrpTxtResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendGrpTxtResult(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }





        public void SendSngTxtResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txSngTxtResult>\n";
                    xmlInfo += "<sent>" + result + "</sent>\n";
                    xmlInfo += "</txSngTxtResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendSngTxtResult(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }











        public void sendAddContactResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddContactResult>\n";
                    xmlInfo += "<added>" + result + "</added>\n";
                    xmlInfo += "</txAddContactResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendAddContactResult(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }



        public void sendEditContactResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txEditContactResult>\n";
                    xmlInfo += "<edited>" + result + "</edited>\n";
                    xmlInfo += "</txEditContactResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendEditContactResult(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }



        public void sendDeleteContactResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteContactResult>\n";
                    xmlInfo += "<deleted>" + result + "</deleted>\n";
                    xmlInfo += "</txDeleteContactResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendDeleteContactResult(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }










        public void sendContactsList(ArrayList contacts)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending Contacts List ");



                    string xmlInfo = xmlStart;
                    xmlInfo += "<txContactsList>\n";
                    for (int i = 0; i < contacts.Count; i++)
                    {
                        xmlInfo += "<contact>" + contacts[i] + "</contact>\n";
                    }
                    xmlInfo += "</txContactsList>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }

        public void sendContactInfo(ArrayList contact)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending Contact Info ");

                    string name = "";
                    string phone = "";
                    string group = "";
                    if (contact.Count > 2)
                    {
                        name = (string)contact[0];
                        phone = (string)contact[1];
                        group = (string)contact[2];
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txContactInfo>\n";
                    xmlInfo += "<contact>";
                    xmlInfo += "<name>" + name + "</name>\n";
                    xmlInfo += "<phone>" + phone + "</phone>\n";
                    xmlInfo += "<group>" + group + "</group>\n";
                    xmlInfo += "</contact>";
                    xmlInfo += "</txContactInfo>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void sendTxtLog(ArrayList list, int inTxts, int outTxts)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending TxtLog ");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txTxtLog>\n";
                    for (int i = 0; i < list.Count; i++)
                    {
                        ArrayList row = (ArrayList)list[i];
                        xmlInfo += "<row>\n";
                        for (int c = 0; c < row.Count; c++)
                        {
                            switch (c)
                            {
                                case 0:
                                    xmlInfo += "<Number>" + row[c] + "</Number>\n";
                                    break;
                                case 1:
                                    xmlInfo += "<Message>" + row[c] + "</Message>\n";
                                    break;
                                case 2:
                                    xmlInfo += "<Sent>" + row[c] + "</Sent>\n";
                                    break;
                                case 3:
                                    xmlInfo += "<Report_Status>" + row[c] + "</Report_Status>\n";
                                    break;
                                case 4:
                                    xmlInfo += "<Modem>" + row[c] + "</Modem>\n";
                                    break;
                            }
                        }
                        xmlInfo += "</row>\n";
                    }
                    xmlInfo += "</txTxtLog>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendTxtLog(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }










        public void sendFailedLog(ArrayList list)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending FailedLog ");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txFailedLog>\n";
                    for (int i = 0; i < list.Count; i++)
                    {
                        ArrayList row = (ArrayList)list[i];
                        xmlInfo += "<row>\n";
                        for (int c = 0; c < row.Count; c++)
                        {
                            switch (c)
                            {
                                case 0:
                                    xmlInfo += "<Number>" + row[c] + "</Number>\n";
                                    break;
                                case 1:
                                    xmlInfo += "<Message>" + row[c] + "</Message>\n";
                                    break;
                                case 2:
                                    xmlInfo += "<Code>" + row[c] + "</Code>\n";
                                    break;
                                case 3:
                                    xmlInfo += "<Error>" + row[c] + "</Error>\n";
                                    break;
                                case 4:
                                    xmlInfo += "<Sent>" + row[c] + "</Sent>\n";
                                    break;
                            }
                        }
                        xmlInfo += "</row>\n";
                    }
                    xmlInfo += "</txFailedLog>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendTxtLog(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }







        public void SendModemCount(int c)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txModemCount>\n";
                    xmlInfo += "<modems>" + c + "</modems>\n";
                    xmlInfo += "</txModemCount>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendModemCount: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void SendDayTotals(int[] t)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDayTotalsResult>\n";
                    xmlInfo += "<outgoing>" + t[0] + "</outgoing>\n";
                    xmlInfo += "<incoming>" + t[1] + "</incoming>\n";
                    xmlInfo += "</txDayTotalsResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendDayTotalsResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }


        public void SendReportResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txReportResult>\n";
                    if (b)
                    {
                        xmlInfo += "<success>yes</success>\n";
                    }
                    else
                    {
                        xmlInfo += "<success>no</success>\n";
                    }
                    xmlInfo += "</txReportResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendReportResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }



































        public void sendJobInfo(ArrayList list)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending job info ");


                    string xmlInfo = xmlStart;

                    if (list.Count > 0)
                    {
                        xmlInfo += "<txGetJobInfoResult>\n";
                        xmlInfo += "<name>" + list[0] + "</name>\n";
                        xmlInfo += "<location>" + list[1] + "</location>\n";
                        xmlInfo += "<file>" + list[2] + "</file>\n";
                        xmlInfo += "<scheduled>" + list[3] + "</scheduled>\n";
                        xmlInfo += "</txGetJobInfoResult>\n";
                        xmlInfo += xmlEnd;
                    }
                    else
                    {
                        xmlInfo += "<txGetJobInfoResult>\n";
                        xmlInfo += "<nojob>none</nojob>\n";
                        xmlInfo += "</txGetJobInfoResult>\n";
                        xmlInfo += xmlEnd;
                    }

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendJobInfo: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }








        public void sendSingleJobResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    Console.WriteLine("sending single job result ");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetSingleJobResult>\n";
                    xmlInfo += "<result>" + result + "</result>\n";
                    xmlInfo += "</txGetSingleJobResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendSingleJobResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }









        public void sendScheduledJobs(ArrayList list, int sch)
        {
            lock (myLock)
            {
                try
                {

                    Console.WriteLine("sending scheduled jobs ");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetJobsResults schedule='" + sch + "'>\n";

                    if (list.Count > 0)
                    {
                        foreach (string[] job in list)
                        {
                            xmlInfo += "<job>\n";
                            xmlInfo += "<name>" + job[0] + "</name>\n";
                            xmlInfo += "<location>" + job[1] + "</location>\n";
                            xmlInfo += "<file>" + job[2] + "</file>\n";
                            xmlInfo += "</job>\n";
                        }
                    }
                    else
                    {
                        xmlInfo += "<nojobs>none</nojobs>\n";
                    }

                    xmlInfo += "</txGetJobsResults>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendScheduledJobs: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }


        public void sendScheduledJobsResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string result;
                    if (b)
                    {
                        result = "yes";
                    }
                    else
                    {
                        result = "no";
                    }

                    Console.WriteLine("sending scheduled jobs result ");

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txGetScheduledJobsResult>\n";
                    xmlInfo += "<result>" + result + "</result>\n";
                    xmlInfo += "</txGetScheduledJobsResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendScheduledJobsResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }









        public void SendAddScheduledJobResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txAddScheduledJobResult>\n";
                    if (b)
                    {
                        xmlInfo += "<added>yes</added>\n";
                    }
                    else
                    {
                        xmlInfo += "<added>no</added>\n";
                    }
                    xmlInfo += "</txAddScheduledJobResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendAddScheduledJobResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }





        public void SendEditScheduledJobResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txEditScheduledJobResult>\n";
                    if (b)
                    {
                        xmlInfo += "<edited>yes</edited>\n";
                    }
                    else
                    {
                        xmlInfo += "<edited>no</edited>\n";
                    }
                    xmlInfo += "</txEditScheduledJobResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendAddScheduledJobResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void SendDeleteScheduledJobResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txDeleteScheduledJobResult>\n";
                    if (b)
                    {
                        xmlInfo += "<deleted>yes</deleted>\n";
                    }
                    else
                    {
                        xmlInfo += "<deleted>no</deleted>\n";
                    }
                    xmlInfo += "</txDeleteScheduledJobResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendDeletedScheduledJobResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }
















        //remove
        public void SendModemLog(string s)
        {
            lock (myLock)
            {
                try
                {
                    string xmlInfo = xmlStart;
                    xmlInfo += "<txModemLog>\n";
                    xmlInfo += "<log>" + s + "</log>\n";
                    xmlInfo += "</txModemLog>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.SendModemCount: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void SendKickConnResult(bool b)
        {
            lock (myLock)
            {
                try
                {
                    string res;
                    if (b)
                    {
                        res = "yes";
                    }
                    else
                    {
                        res = "no";
                    }

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txKickConnResult>\n";
                    xmlInfo += "<kicked>" + res + "</kicked>\n";
                    xmlInfo += "</txKickConnResult>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.KickConnResult: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }






        public void sendSysMsg(string arg0, string arg1)
        {
            lock (myLock)
            {
                try
                {

                    string xmlInfo = xmlStart;
                    xmlInfo += "<txSysMsg>\n";
                    xmlInfo += "<title>" + arg0 + "</title>\n";
                    xmlInfo += "<message>" + arg1 + "</message>\n";
                    xmlInfo += "</txSysMsg>\n";
                    xmlInfo += xmlEnd;

                    lock (mWriter)
                    {
                        mWriter.Write(xmlInfo);
                        mWriter.Flush();
                    }

                }
                catch (Exception e)
                {
                    mCore.doEventLog("SockSender.sendSysMsg(): " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
        }



        



    }
}
