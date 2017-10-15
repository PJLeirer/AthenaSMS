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
using System.Windows.Controls;
using System.Xml;

namespace AthenaConsole
{
    public class SockRecv
    {
        public TcpClient client = null;
        public bool doRun = true;
        private bool athena_obj;

        NetworkStream mStream;


        MainWindow mMainWin;

        char endByt = (char)0X00;



        public SockRecv(TcpClient c, MainWindow w)
        {
            client = c;
            mMainWin = w;
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

                        mMainWin.SocketInUpdater(xdata + "\r\n");

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
                                                //Console.WriteLine("got txUserInfo");
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
                                                        mXmlReader.Read();// end tag
                                                    }
                                                }
                                                if (userId > 0 && userName != null && userLevel > 0)
                                                {
                                                    mMainWin.mSockMan.mSyncUser.assignUserInfo(userId, userName, userLevel);
                                                }
                                                mMainWin.mSockMan.mSockSend.loginEvent.Set();
                                            }





                                            else if(mXmlReader.Name.Trim().Equals("txAddUserResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Trim().Equals("added"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Equals("yes"))
                                                        {
                                                            mMainWin.mSockMan.addedUser = true;
                                                        }
                                                    }
                                                }
                                                mMainWin.mSockMan.mSockSend.addUserEvent.Set();
                                            }




                                            else if (mXmlReader.Name.Trim().Equals("txDeleteUserResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Trim().Equals("deleted"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Equals("yes"))
                                                        {
                                                            mMainWin.mSockMan.deletedUser = true;
                                                        }
                                                    }
                                                }
                                                mMainWin.mSockMan.mSockSend.deleteUserEvent.Set();
                                            }













                                            else if (mXmlReader.Name.Trim().Equals("txChangeUserPassResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Trim().Equals("changed"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Equals("yes"))
                                                        {
                                                            mMainWin.mSockMan.changedPass = true;
                                                        }
                                                    }
                                                }
                                                mMainWin.mSockMan.mSockSend.changePassEvent.Set();
                                            }





                                            else if (mXmlReader.Name.Trim().Equals("txChangeUserLevelResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Trim().Equals("changed"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Equals("yes"))
                                                        {
                                                            mMainWin.mSockMan.changedLevel = true;
                                                        }
                                                    }
                                                }
                                                mMainWin.mSockMan.mSockSend.changeLevelEvent.Set();
                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txUserList"))
                                            {
                                                ArrayList userList = new ArrayList();
                                                mXmlReader.Read(); //start tag
                                                while (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("user"))
                                                    {
                                                        mXmlReader.Read();
                                                        userList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.AllUserUpdater(userList);
                                                mMainWin.mSockMan.mSockSend.userListEvent.Set();

                                            }







                                            else if (mXmlReader.Name.Trim().Equals("txOnlineList"))
                                            {
                                                ArrayList userList = new ArrayList();
                                                mXmlReader.Read(); //start tag
                                                while (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("user"))
                                                    {
                                                        mXmlReader.Read();
                                                        userList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.OnlineUserUpdater(userList);
                                                mMainWin.mSockMan.mSockSend.onlineListEvent.Set();
                                                userList = null;
                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txGetMsgModeResult"))
                                            {
                                                int msgMode = 0;
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("mode"))
                                                    {
                                                        mXmlReader.Read();
                                                        Int32.TryParse(mXmlReader.Value.Trim(), out msgMode); ;
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.MsgModeRadioUpdater(msgMode);
                                                mMainWin.mSockMan.mSockSend.getSysMsgModeEvent.Set();
                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txSetMsgModeResult"))
                                            {
                                                bool didSetMsgMode = false;
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("mode"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didSetMsgMode = true;
                                                        }
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.mSockMan.mSockSend.setSysMsgModeEvent.Set();
                                                if (!didSetMsgMode)
                                                {
                                                    MessageBox.Show("Failed to update", "Athena", MessageBoxButton.OK);
                                                }

                                            }










                                            else if (mXmlReader.Name.Trim().Equals("txAddGroupResult"))
                                            {
                                                bool didAddGroup = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("added"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didAddGroup = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.addGroupEvent.Set();

                                                if (didAddGroup)
                                                {
                                                    new Thread(new ThreadStart(
                                                        delegate()
                                                        {
                                                            mMainWin.mSockMan.mSockSend.GetAllGroups();
                                                        }
                                                      )).Start();
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Add Group", "Athena", MessageBoxButton.OK);
                                                }

                                            }



                                            else if (mXmlReader.Name.Trim().Equals("txDeleteGroupResult"))
                                            {
                                                bool didDelGroup = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("deleted"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didDelGroup = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.deleteGroupEvent.Set();

                                                if (didDelGroup)
                                                {
                                                    new Thread(new ThreadStart(
                                                        delegate()
                                                        {
                                                            mMainWin.mSockMan.mSockSend.GetAllGroups();
                                                        }
                                                      )).Start();
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Delete Group", "Athena", MessageBoxButton.OK);
                                                }
                                            }










                                            else if (mXmlReader.Name.Trim().Equals("txGroupList"))
                                            {
                                                ArrayList groupList = new ArrayList();
                                                mXmlReader.Read(); //start tag
                                                while (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("group"))
                                                    {
                                                        mXmlReader.Read();
                                                        groupList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.GroupsListUpdater(groupList);
                                                mMainWin.EditGroupListUpdater(groupList);
                                                if (mMainWin.mEditContactForm != null)
                                                {
                                                    mMainWin.mEditContactForm.ContactGroupsUpdater(groupList);
                                                }
                                                mMainWin.mSockMan.mSockSend.groupsListEvent.Set();

                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txGrpTxtResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.Name.Equals("sent"))
                                                {
                                                    mXmlReader.Read();
                                                    if (mXmlReader.Value.Equals("yes"))
                                                    {
                                                        MessageBox.Show("Failed to send Group Text!", "Athena", MessageBoxButton.OK);
                                                        mMainWin.mSockMan.sentGroupTxt = true;
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Sent Group Text", "Athena", MessageBoxButton.OK);
                                                    }
                                                }


                                                mMainWin.mSockMan.mSockSend.groupTextEvent.Set();

                                            }





                                            else if (mXmlReader.Name.Trim().Equals("txSngTxtResult"))
                                            {
                                                mXmlReader.Read();
                                                if (mXmlReader.Name.Trim().Equals("sent"))
                                                {
                                                    mXmlReader.Read();
                                                    if (mXmlReader.Value.Equals("yes"))
                                                    {
                                                        mMainWin.mSockMan.sentSingleTxt = true;
                                                        MessageBox.Show("Failed to send text!", "Athena", MessageBoxButton.OK);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Sent Text", "Athena", MessageBoxButton.OK);
                                                    }
                                                }


                                                mMainWin.mSockMan.mSockSend.singleTextEvent.Set();

                                            }




























                                            else if (mXmlReader.Name.Trim().Equals("txAddContactResult"))
                                            {
                                                bool didAddContact = false;
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("added"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didAddContact = true;
                                                        }
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.mSockMan.mSockSend.addContactEvent.Set();


                                                if (!didAddContact)
                                                {
                                                    MessageBox.Show("Failed to add contact!", "Athena", MessageBoxButton.OK);
                                                }
                                                else
                                                {
                                                    new Thread(new ThreadStart(
                                                    delegate()
                                                    {
                                                        mMainWin.mSockMan.mSockSend.GetContacts();
                                                    }
                                                  )).Start();
                                                }

                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txEditContactResult"))
                                            {
                                                bool didEditContact = false;
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("edited"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didEditContact = true;
                                                        }
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.mSockMan.mSockSend.editContactEvent.Set();


                                                if (!didEditContact)
                                                {
                                                    mMainWin.mEditContactForm.lEditContactError.Content = "Failed to edit contact!";
                                                }
                                                else
                                                {
                                                    new Thread(new ThreadStart(
                                                        delegate()
                                                        {
                                                            mMainWin.mSockMan.mSockSend.GetContacts();
                                                        }
                                                      )).Start();
                                                    mMainWin.Dispatcher.Invoke(
                                                    System.Windows.Threading.DispatcherPriority.Normal,
                                                    new Action(
                                                      delegate()
                                                      {
                                                          mMainWin.mEditContactForm.lEditContactError.Content = "Updated Contact!";
                                                      }
                                                  ));
                                                }

                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txDeleteContactResult"))
                                            {
                                                bool didDeleteContact = false;
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("deleted"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didDeleteContact = true;
                                                        }
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.mSockMan.mSockSend.deleteContactEvent.Set();


                                                if (!didDeleteContact)
                                                {
                                                    MessageBox.Show("Failed to delete contact!", "Athena", MessageBoxButton.OK);
                                                }
                                                else
                                                {
                                                    new Thread(new ThreadStart(
                                                    delegate()
                                                    {
                                                        mMainWin.mSockMan.mSockSend.GetContacts();
                                                    }
                                                  )).Start();
                                                }

                                            }








                                            else if (mXmlReader.Name.Trim().Equals("txContactsList"))
                                            {
                                                ArrayList contactsList = new ArrayList();
                                                mXmlReader.Read(); //start tag
                                                while (mXmlReader.IsStartElement())
                                                {

                                                    if (mXmlReader.Name.Equals("contact"))
                                                    {
                                                        mXmlReader.Read();
                                                        contactsList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();// end tag
                                                    }
                                                    mXmlReader.Read(); //start tag?

                                                }

                                                mMainWin.ContactsListUpdater(contactsList);
                                                mMainWin.mSockMan.mSockSend.contactsListEvent.Set();
                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txContactInfo"))
                                            {
                                                string Cname = "";
                                                string Cphone = "";
                                                string Cgroup = "";
                                                mXmlReader.Read(); //start tag
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("contact"))
                                                    {
                                                        mXmlReader.Read();
                                                        while (mXmlReader.IsStartElement())
                                                        {
                                                            if (mXmlReader.Name.Trim().Equals("name"))
                                                            {
                                                                mXmlReader.Read();
                                                                Cname = mXmlReader.Value.Trim();
                                                                mXmlReader.Read();
                                                            }
                                                            if (mXmlReader.Name.Trim().Equals("phone"))
                                                            {
                                                                mXmlReader.Read();
                                                                Cphone = mXmlReader.Value.Trim();
                                                                mXmlReader.Read();
                                                            }
                                                            if (mXmlReader.Name.Trim().Equals("group"))
                                                            {
                                                                mXmlReader.Read();
                                                                Cgroup = mXmlReader.Value.Trim();
                                                                mXmlReader.Read();
                                                            }

                                                            mXmlReader.Read();// read next start element or end
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.getContactInfoEvent.Set();

                                                if (mMainWin.mEditContactForm != null)
                                                {
                                                    mMainWin.mEditContactForm.FieldsUpdater(Cname, Cphone, Cgroup);
                                                }

                                            }










                                            else if (mXmlReader.Name.Trim().Equals("txTxtLog"))
                                            {
                                                ArrayList txtList = new ArrayList();
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    while (mXmlReader.Name.Equals("row"))
                                                    {
                                                        mXmlReader.Read();
                                                        txtList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();
                                                    }
                                                    mXmlReader.Read();
                                                }
                                                mMainWin.OutgoingTextsListUpdater(txtList);
                                                mMainWin.mSockMan.mSockSend.textLogEvent.Set();
                                            }







                                            else if (mXmlReader.Name.Trim().Equals("txFailedLog"))
                                            {
                                                ArrayList failedList = new ArrayList();
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    while (mXmlReader.Name.Equals("row"))
                                                    {
                                                        mXmlReader.Read();
                                                        failedList.Add(mXmlReader.Value.Trim());
                                                        mXmlReader.Read();
                                                    }
                                                    mXmlReader.Read();
                                                }
                                                mMainWin.FailedTextsListUpdater(failedList);
                                                mMainWin.mSockMan.mSockSend.failedLogEvent.Set();
                                            }








                                            else if (mXmlReader.Name.Trim().Equals("txModemCount"))
                                            {
                                                int mdms = -1;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("modems"))
                                                    {
                                                        mXmlReader.Read();
                                                        Int32.TryParse(mXmlReader.Value.Trim(), out mdms);
                                                        mXmlReader.Read();
                                                    }
                                                }
                                                mMainWin.ModemsListUpdater(mdms);
                                                mMainWin.mSockMan.mSockSend.getModemsEvent.Set();
                                            }





                                            


                                            else if (mXmlReader.Name.Trim().Equals("txDayTotalsResult"))
                                            {
                                                int og = 0;
                                                int ic = 0;
                                                mXmlReader.Read();
                                                while (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("outgoing"))
                                                    {
                                                        mXmlReader.Read();
                                                        Int32.TryParse(mXmlReader.Value.Trim(), out og);
                                                        mXmlReader.Read();
                                                    }
                                                    if (mXmlReader.Name.Equals("incoming"))
                                                    {
                                                        mXmlReader.Read();
                                                        Int32.TryParse(mXmlReader.Value.Trim(), out ic);
                                                        mXmlReader.Read();
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.getDayTotalsEvent.Set();

                                                mMainWin.DayTotalsUpdater(og, ic);
                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txReportResult"))
                                            {
                                                bool reportSuccess = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("success"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            reportSuccess = true;
                                                        }
                                                        mXmlReader.Read();
                                                    }
                                                }
                                                string rptmsg = "";
                                                if (reportSuccess)
                                                {
                                                    rptmsg = "Successfully Sent Report!";
                                                }
                                                else
                                                {
                                                    rptmsg = "Failed to Send Report!";
                                                }
                                                mMainWin.Dispatcher.Invoke(
                                                    System.Windows.Threading.DispatcherPriority.Normal,
                                                    new Action(
                                                      delegate()
                                                      {
                                                          MessageBox.Show(rptmsg, "Athena", MessageBoxButton.OK);
                                                      }
                                                  ));

                                                mMainWin.mSockMan.mSockSend.getReportEvent.Set();
                                            }







                                                //remove
                                            else if (mXmlReader.Name.Trim().Equals("txModemLog"))
                                            {
                                                string mLog = "";
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("log"))
                                                    {
                                                        mXmlReader.Read();
                                                        mLog = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }
                                                }
                                                mMainWin.ModemLogListUpdater(mLog);
                                                mMainWin.mSockMan.mSockSend.getModemLogEvent.Set();
                                            }








                                            else if (mXmlReader.Name.Trim().Equals("txGetJobInfoResult"))
                                            {
                                                string jiname = "";
                                                string jilocation = "";
                                                string jifile = "";
                                                int jischedule = -1;

                                                mXmlReader.Read();
                                                while (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("name"))
                                                    {
                                                        mXmlReader.Read();
                                                        jiname = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }
                                                    if (mXmlReader.Name.Equals("location"))
                                                    {
                                                        mXmlReader.Read();
                                                        jilocation = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }
                                                    if (mXmlReader.Name.Equals("file"))
                                                    {
                                                        mXmlReader.Read();
                                                        jifile = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }
                                                    if (mXmlReader.Name.Equals("scheduled"))
                                                    {
                                                        mXmlReader.Read();
                                                        Int32.TryParse(mXmlReader.Value.Trim(), out jischedule);
                                                        mXmlReader.Read();
                                                    }

                                                    mXmlReader.Read(); //start
                                                }


                                                mMainWin.mSockMan.mSockSend.getJobInfoEvent.Set();

                                                if (mMainWin.mEditScheduledJobForm != null)
                                                {
                                                    //mMainWin.ErrorLogUpdater(jiname + ", " + jilocation + ", " + jifile + ", " + jischedule);
                                                    mMainWin.mEditScheduledJobForm.FieldUpdater(jiname, jilocation, jifile, jischedule);
                                                }



                                            }





                                            else if (mXmlReader.Name.Trim().Equals("txGetSingleJobResult"))
                                            {

                                                bool didRunSingleJob = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("result"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didRunSingleJob = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.runSingleJobEvent.Set();

                                                string didRunText;
                                                if (didRunSingleJob)
                                                {
                                                    didRunText = "Running Job";
                                                }
                                                else
                                                {
                                                    didRunText = "Failed to Run Job";
                                                }
                                                MessageBox.Show(didRunText, "Athena", MessageBoxButton.OK);

                                            }



                                            





                                            else if (mXmlReader.Name.Trim().Equals("txGetJobsResults"))
                                            {

                                                int jrSch = -1;

                                                if (mXmlReader.MoveToFirstAttribute())
                                                {
                                                    Int32.TryParse(mXmlReader.Value, out jrSch);
                                                }

                                                mMainWin.ErrorLogUpdater("job results");

                                                string jrName = "";
                                                string jrLocation = "";
                                                string jrFile = "";

                                                ArrayList schJobsList = new ArrayList();

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    while (mXmlReader.Name.Equals("job"))
                                                    {
                                                        // mMainWin.ErrorLogUpdater("job");
                                                        mXmlReader.Read();
                                                        while (mXmlReader.IsStartElement())
                                                        {

                                                            if (mXmlReader.Name.Trim().Equals("name"))
                                                            {
                                                                mXmlReader.Read();
                                                                jrName = mXmlReader.Value.Trim();
                                                                schJobsList.Add(jrName);
                                                                mMainWin.ErrorLogUpdater("jrname: " + jrName);
                                                                mXmlReader.Read();
                                                            }
                                                            else if (mXmlReader.Name.Trim().Equals("location"))
                                                            {
                                                                mXmlReader.Read();
                                                                jrLocation = mXmlReader.Value.Trim();
                                                                mXmlReader.Read();
                                                            }
                                                            else if (mXmlReader.Name.Trim().Equals("file"))
                                                            {
                                                                mXmlReader.Read();
                                                                jrFile = mXmlReader.Value.Trim();
                                                                mXmlReader.Read();
                                                            }
                                                            else if (mXmlReader.Name.Trim().Equals("job"))
                                                            {
                                                                break;
                                                            }
                                                            mXmlReader.Read(); // start or end
                                                        }
                                                        //mXmlReader.Read();
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.getScheduledJobsEvent.Set();


                                                switch (jrSch)
                                                {
                                                    case 3:
                                                        mMainWin.MonthlyJobsListUpdater(schJobsList);
                                                        mMainWin.ErrorLogUpdater("updated monthly jobs");
                                                        break;

                                                    case 2:
                                                        mMainWin.WeeklyJobsListUpdater(schJobsList);
                                                        mMainWin.ErrorLogUpdater("updated weekly jobs");
                                                        break;

                                                    case 1:
                                                        mMainWin.DailyJobsListUpdater(schJobsList);
                                                        mMainWin.ErrorLogUpdater("updated daily jobs");
                                                        break;

                                                    case 0:
                                                        mMainWin.UnscheduledJobsListUpdater(schJobsList);
                                                        mMainWin.ErrorLogUpdater("updated unscheduled jobs " + schJobsList.Count);
                                                        break;

                                                    default://-1
                                                        mMainWin.ErrorLogUpdater("unknown job");
                                                        break;
                                                }


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

                                                mMainWin.mSockMan.mSockSend.runScheduledJobsEvent.Set();

                                                string didRunJobsText;
                                                if (didRunScheduledJobs)
                                                {
                                                    didRunJobsText = "Running Jobs";
                                                }
                                                else
                                                {
                                                    didRunJobsText = "Failed to Run Jobs";
                                                }
                                                MessageBox.Show(didRunJobsText, "Athena", MessageBoxButton.OK);
                                            }







                                            





                                            else if (mXmlReader.Name.Trim().Equals("txAddScheduledJobResult"))
                                            {

                                                bool didAddSchJob = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("added"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didAddSchJob = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.addScheduledJobEvent.Set();

                                                if (didAddSchJob)
                                                {
                                                    MessageBox.Show("Added New Job", "Athena", MessageBoxButton.OK);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Add New Job", "Athena", MessageBoxButton.OK);
                                                }

                                            }






                                            else if (mXmlReader.Name.Trim().Equals("txEditScheduledJobResult"))
                                            {

                                                bool didEditSchJob = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("edited"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didEditSchJob = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.editScheduledJobEvent.Set();

                                                if (didEditSchJob)
                                                {
                                                    MessageBox.Show("Edited New Job", "Athena", MessageBoxButton.OK);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Edit New Job", "Athena", MessageBoxButton.OK);
                                                }
                                            }



                                            else if (mXmlReader.Name.Trim().Equals("txDeleteScheduledJobResult"))
                                            {

                                                bool didDeleteSchJob = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("deleted"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didDeleteSchJob = true;
                                                        }
                                                    }
                                                }

                                                mMainWin.mSockMan.mSockSend.deleteScheduledJobEvent.Set();

                                                if (didDeleteSchJob)
                                                {
                                                    MessageBox.Show("Deleted Job", "Athena", MessageBoxButton.OK);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Failed to Delete Job", "Athena", MessageBoxButton.OK);
                                                }

                                            }





                                            else if (mXmlReader.Name.Trim().Equals("txKickConnResult"))
                                            {
                                                bool didKick = false;
                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("kicked"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("yes"))
                                                        {
                                                            didKick = true;
                                                        }
                                                    }
                                                }
                                                mMainWin.mSockMan.mSockSend.kickConnEvent.Set();
                                                if (!didKick)
                                                {
                                                    MessageBox.Show("Failed to kick user", "Athena", MessageBoxButton.OK);
                                                }
                                            }








                                            else if (mXmlReader.Name.Trim().Equals("txSysMsg"))
                                            {
                                                string sysmsgTitle = "";
                                                string sysmsgMessage = "";
                                                mXmlReader.Read();
                                                while (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("title"))
                                                    {
                                                        mXmlReader.Read();
                                                        sysmsgTitle = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    } if (mXmlReader.Name.Equals("message"))
                                                    {
                                                        mXmlReader.Read();
                                                        sysmsgMessage = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }
                                                    mXmlReader.Read();
                                                }

                                                new Thread(new ThreadStart(
                                                    delegate()
                                                    {
                                                        mMainWin.Dispatcher.Invoke(
                                                            System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action(
                                                              delegate()
                                                              {
                                                                  MessageBox.Show(sysmsgMessage, "Athena " + sysmsgTitle, MessageBoxButton.OK);
                                                              }
                                                          ));
                                                    }
                                                    )).Start();
                                            }







                                                //general usage
                                            //TODO eliminate
                                            else if (mXmlReader.Name.Trim().Equals("txData"))
                                            {

                                                mXmlReader.Read();
                                                if (mXmlReader.IsStartElement())
                                                {
                                                    if (mXmlReader.Name.Equals("sngtext"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.sentSingleTxt = true;
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.sentSingleTxt = false;
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        mMainWin.mSockMan.mSockSend.singleTextEvent.Set();
                                                    }
                                                    else if (mXmlReader.Name.Equals("grptxt"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.sentGroupTxt = true;
                                                            mMainWin.mSockMan.mSockSend.groupTextEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.sentGroupTxt = false;
                                                            mMainWin.mSockMan.mSockSend.groupTextEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    else if (mXmlReader.Name.Equals("changelevel"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.changedLevel = true;
                                                            mMainWin.mSockMan.mSockSend.changeLevelEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.changedLevel = false;
                                                            mMainWin.mSockMan.mSockSend.changeLevelEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    else if (mXmlReader.Name.Equals("changepass"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.changedPass = true;
                                                            mMainWin.mSockMan.mSockSend.changePassEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.changedPass = false;
                                                            mMainWin.mSockMan.mSockSend.changePassEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    else if (mXmlReader.Name.Equals("deleteuser"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.deletedUser = true;
                                                            mMainWin.mSockMan.mSockSend.deleteUserEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.deletedUser = false;
                                                            mMainWin.mSockMan.mSockSend.deleteUserEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    else if (mXmlReader.Name.Equals("adduser"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            mMainWin.mSockMan.addedUser = true;
                                                            mMainWin.mSockMan.mSockSend.addUserEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            mMainWin.mSockMan.addedUser = false;
                                                            mMainWin.mSockMan.mSockSend.addUserEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    else if (mXmlReader.Name.Equals("dailynotice"))
                                                    {
                                                        mXmlReader.Read();
                                                        if (mXmlReader.Value.Trim().Equals("success"))
                                                        {
                                                            Console.WriteLine("succesfully processed daily notices");
                                                            //Program.ranDailyNotice = true;
                                                            string s;
                                                            //doRun = false;
                                                            mMainWin.mSockMan.mSockSend.msgEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("failed to process daily notices");
                                                            //Program.sendMail("Daily Hold Notice Error", "Failed to Process 'Daily Hold Notices'");
                                                            //string s;
                                                            //doRun = false;
                                                            mMainWin.mSockMan.mSockSend.msgEvent.Set();
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }
                                                    mXmlReader.Read();// end tag
                                                }
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
                    mMainWin.ErrorLogUpdater("SockMainRecv.looper: " + e.Message + "\n\n" + e.StackTrace);
                    client.Close();
                    break;
                }
            }
        }//looper
    }
}
