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
    class SockSender
    {

        StreamWriter mWriter;

        public Core mCore;

        string xmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<AthenaObj>\n";
        string xmlEnd = "</AthenaObj>";


        public SockSender(Core core, TcpClient c)
        {
            mCore = core;
            mWriter = new StreamWriter(c.GetStream());
        }

        public void sendUserInfo(int userId, String userName, int userLevel)
        {
            try
            {

                string xmlInfo = xmlStart;
                xmlInfo += "<txUserInfo>\n";
                xmlInfo += "<id>" + userId + "</id>\n";
                xmlInfo += "<name>" + userName + "</name>\n";
                xmlInfo += "<level>" + userLevel + "</level>\n";
                xmlInfo += "</txUserInfo>\n";
                xmlInfo += xmlEnd;

                Console.WriteLine("sending... \n" + xmlInfo);
                mWriter.Write(xmlInfo);
                mWriter.Flush();

            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendUserInfo(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }






        public void sendSysMsg(string arg0, string arg1)
        {
            try
            {

                string xmlInfo = xmlStart;
                xmlInfo += "<txSysMsg>\n";
                xmlInfo += "<title>" + arg0 + "</title>\n";
                xmlInfo += "<message>" + arg1 + "</message>\n";
                xmlInfo += "</txSysMsg>\n";
                xmlInfo += xmlEnd;

                mWriter.Write(xmlInfo);
                mWriter.Flush();

            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendSysMsg(): " + e.Message, 0);
            }
        }



        public void sendData(string tag, string val)
        {
            try
            {

                Console.WriteLine("sending data " + tag + ":" + val);

                string xmlInfo = xmlStart;
                xmlInfo += "<txData>\n";
                xmlInfo += "<" + tag + ">" + val + "</" + tag + ">\n";
                xmlInfo += "</txData>\n";
                xmlInfo += xmlEnd;

                mWriter.Write(xmlInfo);
                mWriter.Flush();
            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendData(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }

        public void sendGroupList()
        {
            try
            {

                Console.WriteLine("sending groupList ");

                String[] groups = mCore.mSqlDb.getGroupList();

                string xmlInfo = xmlStart;
                xmlInfo += "<txGroupList>\n";
                for (int i = 0; i < groups.Length; i++)
                {
                    xmlInfo += "<group>" + groups[i] + "</group>\n";
                }
                xmlInfo += "</txGroupList>\n";
                xmlInfo += xmlEnd;

                mWriter.Write(xmlInfo);
                mWriter.Flush();

            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }

        public void sendUserList()
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

                mWriter.Write(xmlInfo);
                mWriter.Flush();

            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendUserList(): " + e.Message + "\r\n" + e.StackTrace, 0);
            }
        }



        public void sendTxtLog(ArrayList list, int inTxts, int outTxts)
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

                mWriter.Write(xmlInfo);
                mWriter.Flush();

            }
            catch (Exception e)
            {
                mCore.doEventLog("SockSender.sendTxtLog(): " + e.Message, 0);
            }
        }


    }
}
